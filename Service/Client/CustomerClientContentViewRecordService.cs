using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Client;

[Service(ServiceType.Scoped)]
public class CustomerClientContentViewRecordService {
    private readonly ISession session;

    public CustomerClientContentViewRecordService(ISession session) {
        this.session = session;
    }
    
    
}