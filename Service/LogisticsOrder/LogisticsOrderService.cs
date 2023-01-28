using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderService {
    private readonly ISession session;

    public LogisticsOrderService(ISession session) {
        this.session = session;
    }
}