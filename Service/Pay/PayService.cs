using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PayService {
    private readonly ISession session;

    public PayService(ISession session) {
        this.session = session;
    }

    public string GenerateOutTradeNo(PayType payType) {
        if (payType == PayType.Wechat) {
            return $"{WechatPayConfig.MchId}{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        throw new Exception("还不支持的支付方式！");
    }
}

// return string.Format("{0}{1}{2}", WeChatConfig2.MchId, DateTime.Now.ToString("yyyyMMddHHmmssfff"), random.Next(9999).ToString("0000"));