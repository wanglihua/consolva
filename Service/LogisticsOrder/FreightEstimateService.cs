using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class FreightEstimateService {
    private readonly ISession session;

    public FreightEstimateService(ISession session) {
        this.session = session;
    }
}