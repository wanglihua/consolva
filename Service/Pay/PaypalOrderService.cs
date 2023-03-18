using NLog;
using shunda.Core.Service;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    
    
}