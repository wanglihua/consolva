using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 客户充值优惠设置 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerChargeAwardSettingService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerChargeAwardSettingService(ISession session) {
        this.session = session;
    }
}