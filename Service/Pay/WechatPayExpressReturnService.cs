using NLog;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayExpressReturnService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatPayExpressReturnService(ISession session) {
        this.session = session;
    }
    
    
}