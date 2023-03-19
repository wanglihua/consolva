using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Customer;
using shunda.Service.LogisticsOrder;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class WriteOffFeeService {
    private readonly ISession session;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;

    public WriteOffFeeService(ISession session,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService) {
        this.session = session;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
    }

    public void AddWriteOffFee(LogisticsOrderInfo logisticsOrder, CustomerInfo customer, decimal amount, int feeReasonId, int userId) {
        ManualFeeReasonInfo? manualFeeReason = session.Get<ManualFeeReasonInfo>(feeReasonId);
        if (manualFeeReason == null) {
            throw new Exception($"指定的核销原因#{feeReasonId}不存在！");
        }

        string reasonContent = manualFeeReason.Content ?? string.Empty;

        UserInfo? user = session.Get<UserInfo>(userId);
        if (user == null) {
            throw new Exception($"指定的操作用户#{userId}不存在！");
        }

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo();

        orderPay.Order = logisticsOrder;
        orderPay.OrderCode = logisticsOrder.Code;
        orderPay.Customer = customer;
        orderPay.OutTradeNo = string.Empty;
        orderPay.PayPalPaymentId = string.Empty;
        orderPay.Status = PayStatus.Paid;
        orderPay.PayType = PayType.WriteOff;
        orderPay.Amount = amount;
        orderPay.WriteOffReasonId = feeReasonId;
        orderPay.WriteOffReasonContent = reasonContent;
        orderPay.User = user;

        session.Save(orderPay);
        session.Flush();

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderPay.Order.Id);

        session.Flush();

        string orderActionDesc = $"金额:{orderPay.Amount}, 原因:{reasonContent}";

        // 这个方法一般只做新增不做修改的，写日志的时候就当是做新增了
        logisticsOrderActionService.NewActionUser(orderPay.Order.Id, LogisticsOrderActionType.WriteOffFee, orderActionDesc, userId);

        // 会员积分
        customerScoreService.AddScore(orderPay.Id);
    }
}