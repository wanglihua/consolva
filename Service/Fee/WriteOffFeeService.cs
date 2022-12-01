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
public class WriteOffFeeService
{
    private ISession session;
    private LogisticsOrderFeeService logisticsOrderFeeService;
    private LogisticsOrderActionService logisticsOrderActionService;
    private CustomerScoreService customerScoreService;

    public WriteOffFeeService(ISession session,
        LogisticsOrderFeeService logisticsOrderFeeService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService)
    {
        this.session = session;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
    }

    public void AddWriteOffFee(LogisticsOrderInfo logisticsOrder, CustomerInfo customer, decimal amount, int feeReasonId, int userId)
    {
        ManualFeeReasonInfo? manualFeeReason = session.Get<ManualFeeReasonInfo>(feeReasonId);
        if (manualFeeReason == null)
        {
            throw new Exception($"指定的核销原因#{feeReasonId}不存在！");
        }

        string reasonContent = manualFeeReason.Content ?? string.Empty;

        UserInfo? user = session.Get<UserInfo>(userId);
        if (user == null)
        {
            throw new Exception($"指定的操作用户#{userId}不存在！");
        }

        LogisticsOrderPayInfo logisticsOrderPay = new LogisticsOrderPayInfo();

        logisticsOrderPay.Order = logisticsOrder;
        logisticsOrderPay.Customer = customer;

        logisticsOrderPay.OutTradeNo = string.Empty;
        logisticsOrderPay.PayPalPaymentId = string.Empty;
        logisticsOrderPay.Status = PayStatus.Paid;
        logisticsOrderPay.PayType = PayType.WriteOff;
        logisticsOrderPay.Amount = amount;
        logisticsOrderPay.WriteOffReasonId = feeReasonId;
        logisticsOrderPay.WriteOffReasonContent = reasonContent;
        logisticsOrderPay.User = user;

        session.Save(logisticsOrderPay);
        session.Flush();

        logisticsOrderFeeService.SumFeeAndUpdatePayStatus(logisticsOrderPay.Order);

        session.Flush();

        string orderActionDesc = $"金额:{logisticsOrderPay.Amount}, 原因:{reasonContent}";

        // 这个方法一般只做新增不做修改的，写日志的时候就当是做新增了
        logisticsOrderActionService.NewActionUser(logisticsOrderPay.Order.Id, LogisticsOrderActionType.WriteOffFee, orderActionDesc, userId);

        // 会员积分
        customerScoreService.AddScore(logisticsOrderPay.Id);
    }
}