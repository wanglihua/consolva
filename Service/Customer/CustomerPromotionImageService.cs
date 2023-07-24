using shunda.Core.Service;
using shunda.Domain;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

/// <summary>
/// 客户推广图片 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerPromotionImageService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionImageService(ISession session) {
        this.session = session;
    }
}