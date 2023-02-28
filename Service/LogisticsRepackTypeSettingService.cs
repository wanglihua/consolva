using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 打包类型设置 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsRepackTypeSettingService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsRepackTypeSettingService(ISession session) {
        this.session = session;
    }
}