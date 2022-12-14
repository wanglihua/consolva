using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateSelectExpressService
{
    private readonly ISession session;

    public LogisticsOrderCreateSelectExpressService(ISession session)
    {
        this.session = session;
    }
}