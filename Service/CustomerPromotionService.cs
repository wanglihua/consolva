using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 客户推广 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerPromotionService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionService(ISession session) {
        this.session = session;
    }
}