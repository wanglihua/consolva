using NLog;
using shunda.Core.Service;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    
    
}