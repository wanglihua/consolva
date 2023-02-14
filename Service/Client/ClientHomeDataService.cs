using shunda.Core.Service;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Service.Client;

[Service(ServiceType.Scoped)]
public class ClientHomeDataService {
    private readonly ISession session;

    public ClientHomeDataService(ISession session) {
        this.session = session;
    }

    public ClientHomeDataInfo GetCustomerClientHomeData(int customerId) {
        string sql = $"CALL client_home_data({customerId})";
        return session.CreateSQLQuery(sql)
            .AddEntity(typeof(ClientHomeDataInfo))
            .UniqueResult<ClientHomeDataInfo>();
    }
}