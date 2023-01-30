using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

/// <summary>
/// 物流方式物品类型 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsModeGoodsTypeService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeGoodsTypeService(ISession session) {
        this.session = session;
    }
}