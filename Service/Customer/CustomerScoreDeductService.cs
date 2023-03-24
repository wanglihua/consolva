using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerScoreDeductService {
    private readonly ISession session;
    private readonly LogisticsOrderFeeSumService logisticsOrderFeeSumService;

    public CustomerScoreDeductService(
        ISession session,
        LogisticsOrderFeeSumService logisticsOrderFeeSumService
    ) {
        this.session = session;
        this.logisticsOrderFeeSumService = logisticsOrderFeeSumService;
    }

    /// <summary>
    /// 计算订单可抵扣积分数量
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="baseAmount">基础费用，一般是非促销渠道物流费用</param>
    public CustomerScoreDeductResult GetDeduction(int orderId, decimal baseAmount) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        NHibernateUtil.Initialize(order.Customer);

        var customer = order.Customer;

        if (customer.ValidScore < 0) {
            throw new ValidationException($"客户#{customer.Id}当前积分数不合法，操作失败！");
        }

        CustomerScoreRuleInfo? scoreToBalanceRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.Type == CustomerScoreType.ScoreToBalance)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (scoreToBalanceRule == null) {
            throw new Exception("积分兑换余额规则未设置！");
        }

        if (scoreToBalanceRule.Score <= 0) {
            throw new Exception("积分兑换余额规则，兑换积分设置小于等于零，不符合要求！");
        }

        if (scoreToBalanceRule.Amount <= 0) {
            throw new Exception("积分兑换余额规则，兑换金额设置小于等于零，不符合要求！");
        }

        // 将非促销渠道费用转成积分数量
        decimal feeNeedScore = Math.Round((baseAmount / scoreToBalanceRule.Amount) * scoreToBalanceRule.Score, 2);
        // 客户可用积分
        decimal customerScore = order.Customer.ValidScore;
        // 本次支付可用积分
        decimal score = Math.Min(feeNeedScore, customerScore);
        // 本地支付可抵扣余额
        decimal amount = Math.Round((score / scoreToBalanceRule.Score) * scoreToBalanceRule.Amount, 2);

        return new CustomerScoreDeductResult {
            Score = score,
            Amount = amount,
        };
    }

    /// <summary>
    /// 执行实际的抵扣
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="baseAmount">基础费用，一般是非促销渠道物流费用</param>
    public void Deduct(int orderId, decimal baseAmount) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        NHibernateUtil.Initialize(order.Customer);

        var customer = order.Customer;

        var deductResult = GetDeduction(orderId, baseAmount);

        customer.ValidScore -= deductResult.Score;
        session.Update(customer);

        // 插入积分抵扣费用，负的费用
        var deductFee = new LogisticsOrderFeeInfo {
            Customer = order.Customer,
            Order = order,
            OrderCode = order.Code,
            GoodsType = LogisticsGoodsType.None,
            Type = LogisticsOrderFeeType.ScoreDeductFee,
            Description = $"积分抵扣 {deductResult.Score}分",
            Fee = -deductResult.Amount
        };
        session.Save(deductFee);

        // 记录积分变动日志
        CustomerScoreLogInfo scoreLog = new CustomerScoreLogInfo {
            Customer = customer,
            ScoreType = CustomerScoreType.Deduct,
            Score = -deductResult.Score,
            Amount = deductResult.Amount,
            IsValidScore = true, // 有效积分才能抵扣
            Description = $"订单{order.Code}",
        };
        session.Save(scoreLog);

        session.Flush();
    }
}