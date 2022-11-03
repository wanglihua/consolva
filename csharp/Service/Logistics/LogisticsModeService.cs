using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsModeService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    // private LogisticsExpressService logisticsExpressService = null;
    // private LogisticsFeeService logisticsFeeService = null;

    public LogisticsModeService(
        ISession session
        // LogisticsExpressService logisticsExpressService,
        // LogisticsFeeService logisticsFeeService
    )
    {
        this.session = session;
        // this.logisticsExpressService = logisticsExpressService;
        // this.logisticsFeeService = logisticsFeeService;
    }
}