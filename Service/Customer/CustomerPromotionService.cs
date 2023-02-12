using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

// todo a 先放一个空的用于编译通过
[Service(ServiceType.Scoped)]
public class CustomerPromotionService {
    private readonly ISession session;

    public CustomerPromotionService(ISession session) {
        this.session = session;
    }

    public void NewFellow(string openId, string unionId, int sceneId) {
    }
}