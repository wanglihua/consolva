using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderFeeService {
    private readonly ISession session;

    public LogisticsOrderFeeService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 计算订单物流费用，并返回计算结果
    /// </summary>
    public LogisticsFeeResult Calculate(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        return new LogisticsFeeResult {
        };
    }
}