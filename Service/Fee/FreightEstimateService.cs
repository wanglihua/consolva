using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class FreightEstimateService {
    private readonly ISession session;

    public FreightEstimateService(ISession session) {
        this.session = session;
    }
}