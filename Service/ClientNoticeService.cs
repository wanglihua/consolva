using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 客户端公告 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ClientNoticeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ClientNoticeService(ISession session) {
        this.session = session;
    }
}