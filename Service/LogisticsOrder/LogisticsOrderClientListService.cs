using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderClientListService {
    private readonly ISession session;
    public LogisticsOrderClientListService(ISession session) {
        this.session = session;
    }
    
    
    
}