using NHibernate;
using NLog;
using shunda.Core.Service;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsModeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeService(ISession session) {
        this.session = session;
    }
}