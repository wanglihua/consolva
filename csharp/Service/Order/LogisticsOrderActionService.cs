using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Service.Order;

[Service(ServiceType.Scoped)]
public class LogisticsOrderActionService
{
    private ISession session;

    public LogisticsOrderActionService(ISession session)
    {
        this.session = session;
    }

    #region 订单状态和支付状态都没变动

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, string.Empty, 0, 0);
    }

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType, string actionDesc)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, actionDesc, 0, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, int userId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, string.Empty, userId, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, string actionDesc, int userId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, actionDesc, userId, 0);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, int customerId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, string.Empty, 0, customerId);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, string actionDesc, int customerId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, actionDesc, 0, customerId);
    }

    #endregion

    #region 订单状态有变动和支付状态没变动

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, string.Empty, 0, 0);
    }

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, string actionDesc)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, actionDesc, 0, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, int userId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, string.Empty, userId, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, string actionDesc, int userId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, actionDesc, userId, 0);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, int customerId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, string.Empty, 0, customerId);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, string actionDesc, int customerId)
    {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, order.PayStatus, order.PayStatus, actionDesc, 0, customerId);
    }

    #endregion
    
    #region 订单状态和支付状态都变动

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, string.Empty, 0, 0);
    }

    public void NewActionWithoutUserCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, string actionDesc)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, actionDesc, 0, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, int userId)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, string.Empty, userId, 0);
    }

    public void NewActionUser(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, string actionDesc, int userId)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, actionDesc, userId, 0);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, int customerId)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, string.Empty, 0, customerId);
    }

    public void NewActionCustomer(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, string actionDesc, int customerId)
    {
        NewAction(orderId, actionType, orderStatusBefore, orderStatusAfter, payStatusBefore, payStatusAfter, actionDesc, 0, customerId);
    }

    #endregion

    public void NewAction(int orderId, LogisticsOrderActionType actionType, LogisticsOrderStatus orderStatusBefore, LogisticsOrderStatus orderStatusAfter, PayStatus payStatusBefore, PayStatus payStatusAfter, string actionDesc, int userId, int customerId)
    {
        session.Flush(); // 可能业务对象（订单）的改变（主要是 新建保存）还没提交到数据库，这里提交确认一下

        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new Exception($"指定的订单#{orderId}不存在！");
        }

        if (actionType == null || actionType == LogisticsOrderActionType.None)
        {
            throw new Exception("订单日志类型未指定！");
        }

        if (orderStatusBefore == null)
        {
            orderStatusBefore = LogisticsOrderStatus.None;
        }

        if (orderStatusAfter == null)
        {
            orderStatusAfter = LogisticsOrderStatus.None;
        }

        if (payStatusBefore == null)
        {
            payStatusBefore = PayStatus.None;
        }

        if (payStatusAfter == null)
        {
            payStatusAfter = PayStatus.None;
        }

        if (string.IsNullOrWhiteSpace(actionDesc) == true)
        {
            actionDesc = string.Empty;
        }

        UserInfo? user = null;
        if (userId > 0)
        {
            user = session.Get<UserInfo>(userId);
        }

        CustomerInfo? customer = null;
        if (customerId > 0)
        {
            customer = session.Get<CustomerInfo>(customerId);
        }

        LogisticsOrderActionInfo orderAction = new LogisticsOrderActionInfo();
        orderAction.Order = order;
        orderAction.User = user;
        orderAction.Customer = customer;
        orderAction.Type = actionType;
        orderAction.OrderStatusBefore = orderStatusBefore;
        orderAction.OrderStatusAfter = orderStatusAfter;
        orderAction.PayStatusBefore = payStatusBefore;
        orderAction.PayStatusAfter = payStatusAfter;
        orderAction.ActionDesc = actionDesc;

        session.Save(orderAction);
        session.Flush();
    }
}