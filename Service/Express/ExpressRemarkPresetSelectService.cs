using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

/// <summary>
/// 快递单备注预设 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressRemarkPresetSelectService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ExpressRemarkPresetSelectService(ISession session) {
        this.session = session;
    }
}