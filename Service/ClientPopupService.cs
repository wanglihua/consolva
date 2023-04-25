using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 客户端弹窗 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ClientPopupService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ClientPopupService(ISession session) {
        this.session = session;
    }
}