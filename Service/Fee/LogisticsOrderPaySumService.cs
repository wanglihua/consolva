using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderPaySumService {
    private readonly ISession session;

    public LogisticsOrderPaySumService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 合计费用表
    /// </summary>
    /// <param name="logisticsOrder"></param>
    /// <returns>合计的费用</returns>
    private decimal SumFee(LogisticsOrderInfo logisticsOrder) {
        IList<decimal> feeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == logisticsOrder.Id)
            .Select(e => e.Fee)
            .ToList();

        decimal totalFee = 0m;
        foreach (var fee in feeList) {
            totalFee += fee;
        }

        return totalFee;
    }

    /// <summary>
    /// 合计已支付费用
    /// </summary>
    /// <param name="logisticsOrder"></param>
    /// <returns></returns>
    private decimal SumPay(LogisticsOrderInfo logisticsOrder) {
        IList<decimal> paidList = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.Order.Id == logisticsOrder.Id && e.Status == PayStatus.Paid) // 一定要加上已支付状态
            .Select(e => e.Amount)
            .ToList();

        decimal totalPaid = 0m;
        foreach (var paid in paidList) {
            totalPaid += paid;
        }

        return totalPaid;
    }

    /// <summary>
    /// 合计费用表数据并更新支付状态
    /// </summary>
    /// <param name="logisticsOrder"></param>
    public void SumFeeAndUpdatePayStatus(LogisticsOrderInfo logisticsOrder) {
        decimal totalFee = SumFee(logisticsOrder);
        decimal totalPaid = SumPay(logisticsOrder);

        logisticsOrder.OrderAmount = totalFee;
        logisticsOrder.PaidAmount = totalPaid;

        if (logisticsOrder.PaidAmount > 0 && logisticsOrder.PaidAmount < logisticsOrder.OrderAmount) {
            if (logisticsOrder.PayStatus != PayStatus.PartPaid) {
                logisticsOrder.PayTime = DateTime.Now;
                logisticsOrder.PayStatus = PayStatus.PartPaid;
            }
        }

        if (logisticsOrder.OrderAmount > 0 && logisticsOrder.PaidAmount >= logisticsOrder.OrderAmount) {
            if (logisticsOrder.PayStatus != PayStatus.Paid) {
                logisticsOrder.PayTime = DateTime.Now;
                logisticsOrder.PayStatus = PayStatus.Paid;
            }
        }

        if (logisticsOrder.PaidAmount <= 0 && logisticsOrder.OrderAmount > 0) {
            logisticsOrder.PayTime = null;
            logisticsOrder.PayStatus = PayStatus.NotPaid;
        }

        session.Update(logisticsOrder);
        session.Flush();
    }
}