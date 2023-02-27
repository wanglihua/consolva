using shunda.Core.Service;
using shunda.Core.WechatPay;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayChargeService {
    private readonly ISession session;

    public WechatPayChargeService(ISession session) {
        this.session = session;
    }

    public WechatPayData Create() {
        return new WechatPayData();
    }

    public void Notify() {
    }
}