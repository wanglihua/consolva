using NHibernate;
using shunda.Core;
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
    /// <returns>合计的费用</returns>
    private decimal SumFee(int orderId) {
        var feeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        decimal totalFee = 0m;
        foreach (var fee in feeList) {
            totalFee += fee.Fee;
        }

        return totalFee;
    }

    /// <summary>
    /// 合计已支付费用
    /// </summary>
    /// <returns>已支付金额合计</returns>
    private decimal SumPay(int orderId) {
        var paidList = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.Order.Id == orderId
                        && e.Status == PayStatus.Paid) // 一定要加上已支付状态
            .ToList();

        decimal totalPaid = 0m;
        foreach (var paid in paidList) {
            totalPaid += paid.Amount;
        }

        return totalPaid;
    }

    /// <summary>
    /// 合计费用表数据并更新支付状态
    /// </summary>
    public void SumFeeAndUpdatePayStatus(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        decimal totalFee = SumFee(orderId);
        decimal totalPaid = SumPay(orderId);

        order.OrderAmount = totalFee;
        order.PaidAmount = totalPaid;

        if (order.PaidAmount > 0 && order.PaidAmount < order.OrderAmount) {
            if (order.PayStatus != PayStatus.PartPaid) {
                order.PayTime = DateTime.Now;
                order.PayStatus = PayStatus.PartPaid;
            }
        }

        if (order.OrderAmount > 0 && order.PaidAmount >= order.OrderAmount) {
            if (order.PayStatus != PayStatus.Paid) {
                order.PayTime = DateTime.Now;
                order.PayStatus = PayStatus.Paid;
            }
        }

        if (order.PaidAmount <= 0 && order.OrderAmount > 0) {
            order.PayTime = null;
            order.PayStatus = PayStatus.NotPaid;
        }

        session.Update(order);
        session.Flush();
    }
}