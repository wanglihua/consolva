using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectService
{
    private readonly ISession session;

    public LogisticsOrderCreateSelectService(ISession session)
    {
        this.session = session;
    }

    /// <summary>
    /// 标记订单创建选择失效
    /// </summary>
    /// <param name="customerId"></param>
    /// <exception cref="ValidationException"></exception>
    public void MakeInvalid(int customerId)
    {
        var orderCreateSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (orderCreateSelect == null)
        {
            throw new ValidationException($"指定的订单创建选择记录#{customerId}不存在！");
        }

        orderCreateSelect.Valid = false;

        session.Update(orderCreateSelect);
        session.Flush();
    }
}