using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderManualFeeService
{
    private ISession session;
    // private LogisticsOrderFeeService logisticsOrderFeeService; todo a
    // private LogisticsOrderActionService logisticsOrderActionService; todo a

    public LogisticsOrderManualFeeService(ISession session)
        // LogisticsOrderFeeService logisticsOrderFeeService,
        // LogisticsOrderActionService logisticsOrderActionService)
    {
        this.session = session;
        // this.logisticsOrderFeeService = logisticsOrderFeeService;
        // this.logisticsOrderActionService = logisticsOrderActionService;
    }

    /// <summary>
    /// 手工增加费用
    /// </summary>
    /// <param name="logisticsOrder"></param>
    /// <param name="customer"></param>
    /// <param name="amount">这里是正的费用</param>
    /// <param name="manualReasonId"></param>
    /// <param name="description"></param>
    /// <param name="userId"></param>
    public void AddManualIncreaseFee(LogisticsOrderInfo logisticsOrder, CustomerInfo customer, decimal amount, int manualReasonId, string description, int userId)
    {
        LogisticsOrderFeeInfo logisticsOrderFee = new LogisticsOrderFeeInfo();

        ManualFeeReasonInfo? manualReason = null;
        if (manualReasonId > 0)
        {
            manualReason = session.Get<ManualFeeReasonInfo>(manualReasonId);
        }

        string reasonContent = manualReason?.Content ?? string.Empty;

        UserInfo? user = null;
        if (userId > 0)
        {
            user = session.Get<UserInfo>(userId);
        }

        logisticsOrderFee.Order = logisticsOrder;
        logisticsOrderFee.Customer = customer;
        logisticsOrderFee.GoodsType = LogisticsGoodsType.None;
        logisticsOrderFee.Type = LogisticsOrderFeeType.ManualIncrease;
        logisticsOrderFee.Fee = amount;
        logisticsOrderFee.Description = description ?? ""; // 这个字段是非空的
        logisticsOrderFee.ManualReason = manualReason;
        logisticsOrderFee.ManualReasonContent = reasonContent;
        logisticsOrderFee.ManualUser = user;

        session.Save(logisticsOrderFee);
        session.Flush();

        // logisticsOrderFeeService.SumFeeAndUpdatePayStatus(logisticsOrder); todo a

        string orderActionDesc = $"金额:{amount}，原因:{reasonContent}，描述:{description ?? ""}";
        // logisticsOrderActionService.NewAction(logisticsOrder, LogisticsOrderActionType.ManualIncreaseFee, orderActionDesc); todo a
    }

    /// <summary>
    /// 手工减少费用
    /// </summary>
    /// <param name="logisticsOrder"></param>
    /// <param name="customer"></param>
    /// <param name="amount">这里是正的费用，保存时改成负的</param>
    /// <param name="manualReasonId"></param>
    /// <param name="description"></param>
    /// <param name="userId"></param> 
    public void AddManualDecreaseFee(LogisticsOrderInfo logisticsOrder, CustomerInfo customer, decimal amount, int manualReasonId, string description, int userId)
    {
        LogisticsOrderFeeInfo logisticsOrderFee = new LogisticsOrderFeeInfo();

        ManualFeeReasonInfo? manualReason = null;
        if (manualReasonId > 0)
        {
            manualReason = session.Get<ManualFeeReasonInfo>(manualReasonId);
        }

        string reasonContent = manualReason?.Content ?? string.Empty;

        UserInfo? user = null;
        if (userId > 0)
        {
            user = session.Get<UserInfo>(userId);
        }

        logisticsOrderFee.Order = logisticsOrder;
        logisticsOrderFee.Customer = customer;
        logisticsOrderFee.GoodsType = LogisticsGoodsType.None;
        logisticsOrderFee.Type = LogisticsOrderFeeType.ManualDecrease;
        logisticsOrderFee.Fee = -amount; // 存储的时候，就存成是负的了，否则订单费用计算的时候会出错
        logisticsOrderFee.Description = description ?? ""; // 这个字段是非空的
        logisticsOrderFee.ManualReason = manualReason;
        logisticsOrderFee.ManualReasonContent = reasonContent;
        logisticsOrderFee.ManualUser = user;

        session.Save(logisticsOrderFee);
        session.Flush();

        // logisticsOrderFeeService.SumFeeAndUpdatePayStatus(logisticsOrder); todo a

        string orderActionDesc = $"金额:{amount}，原因:{reasonContent}，描述:{description ?? ""}";
        // logisticsOrderActionService.NewAction(logisticsOrder, LogisticsOrderActionType.ManualDecreaseFee, orderActionDesc);  todo a
    }
}