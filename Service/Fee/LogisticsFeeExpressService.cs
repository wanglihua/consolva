using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsFeeExpressService {
    private readonly ISession session;
    private readonly FeeCalculateLogService feeCalculateLogService;

    public LogisticsFeeExpressService(
        ISession session,
        FeeCalculateLogService feeCalculateLogService) {
        this.session = session;
        this.feeCalculateLogService = feeCalculateLogService;
    }

    public decimal GetFee(decimal totalWeightKg, int logisticsModeId, FeeMessageType feeMessageType) {
        LogisticsModeInfo logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);
        if (logisticsMode == null) {
            throw new Exception("指定的物流方式不存在！");
        }

        feeCalculateLogService.AddLog($"按跨国快递费率计算");

        IList<LogisticsFeeRateExpressInfo> feeRateExpressList = session
            .Query<LogisticsFeeRateExpressInfo>()
            .Where(e => e.LogisticsMode.Id == logisticsModeId)
            .ToList();

        if (feeRateExpressList.Count > 1) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持当前物品重量！SD0060");
            } else {
                throw new LogisticsFeeException($"跨国快递物流方式{logisticsMode.Name}的费率设置多于一个，费率设置有问题！");
            }
        }

        if (feeRateExpressList.Count == 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持当前物品重量！SD0070");
            } else {
                throw new LogisticsFeeException($"跨国快递物流方式{logisticsMode.Name}中没有对应的费率设置！");
            }
        }

        LogisticsFeeRateExpressInfo feeRateExpress = feeRateExpressList[0];

        decimal startPrice = 0m;
        if (feeRateExpress.StartPrice.HasValue) {
            startPrice = feeRateExpress.StartPrice.Value;
        }

        // 首重免费
        if (feeRateExpress.StartFreeMinWeight != null // 不为空
            && feeRateExpress.StartFreeMinWeight.Value > 0 // 不为零 和 负值，正确的正值
            && feeRateExpress.StartFreeMinWeight.Value <= totalWeightKg) // 总重量超首重免费最小重量
        {
            startPrice = 0;
        }

        decimal startWeight = 0m;
        if (feeRateExpress.StartWeight.HasValue) {
            startWeight = feeRateExpress.StartWeight.Value;
        }

        decimal stepPrice = 0m;
        if (feeRateExpress.StepPrice.HasValue) {
            stepPrice = feeRateExpress.StepPrice.Value;
        }

        decimal stepWeight = 0m;
        if (feeRateExpress.StepWeight.HasValue) {
            stepWeight = feeRateExpress.StepWeight.Value;
        }

        if (stepWeight == 0m) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持当前物品重量！SD0080");
            } else {
                throw new LogisticsFeeException($"跨国快递物流方式{logisticsMode.Name}费率设置有问题，其中续重阶梯为0！");
            }
        }

        // 重量满减
        if ((feeRateExpress.WeightBreakStart != null && feeRateExpress.WeightBreakStart.Value > 0)
            && (feeRateExpress.WeightBreakDiscount != null && feeRateExpress.WeightBreakDiscount.Value > 0)) {
            decimal weightBreakStart = feeRateExpress.WeightBreakStart.Value;
            decimal weightBreakDiscount = feeRateExpress.WeightBreakDiscount.Value;

            if (totalWeightKg >= weightBreakStart) {
                totalWeightKg = totalWeightKg - weightBreakDiscount;
            }

            if (totalWeightKg < 0) // 一般不会发生
            {
                totalWeightKg = 0;
            }
        }

        // 按公斤计费，这个逻辑暂且放在 重量满减 后面
        if (feeRateExpress.FeeByKilogram == true) {
            decimal totalWeightKgWholeNumber = Math.Floor(totalWeightKg);
            if (totalWeightKg - totalWeightKgWholeNumber > 0) {
                totalWeightKg = totalWeightKgWholeNumber + 1; // 不满一公斤按一公斤计算
            }
        }

        // 阶梯费用
        decimal stepCount = 0m;
        if (totalWeightKg < startWeight) {
            stepCount = 0m;
        } else {
            stepCount = Math.Ceiling(Math.Abs(totalWeightKg - startWeight) / stepWeight); // 不足部分算一个阶梯
        }

        feeCalculateLogService.AddLog($"费率: 起步重量 {startWeight}, 起步价格 {startPrice}, 阶梯重量 {startWeight}, 阶梯价格 {startPrice}, 首重免费最小总重 {feeRateExpress.StartFreeMinWeight}");
        feeCalculateLogService.AddLog($"计费阶梯数量: {stepCount}");

        decimal totalFee = startPrice + stepCount * stepPrice;

        feeCalculateLogService.AddLog($"计算得费用: {totalFee}");

        return totalFee;
    }
}