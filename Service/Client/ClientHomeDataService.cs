using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Client;

[Service(ServiceType.Scoped)]
public class ClientHomeDataService {
    private readonly ISession session;

    public ClientHomeDataService(ISession session) {
        this.session = session;
    }
}