using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

/// <summary>
/// 物品类型介绍 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsGoodsTypeSettingService
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsGoodsTypeSettingService(ISession session)
    {
        this.session = session;
    }
}