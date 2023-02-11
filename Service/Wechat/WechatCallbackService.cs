using shunda.Core.Service;
using shunda.Domain.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatCallbackService {
    private static bool trackingEnabled = false;

    private readonly ISession session;

    public WechatCallbackService(ISession session) {
        this.session = session;
    }

    public bool TrackingEnabled {
        get { return trackingEnabled; }
        set { trackingEnabled = value; }
    }

    public void AddWechatOfficialAccountGetCallbackRecord(string param, string body, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatOfficialAccount, WechatCallbackHttpMethod.Get, param, body, responseContent);
    }

    public void AddWechatOfficialAccountPostCallbackRecord(string param, string body, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatOfficialAccount, WechatCallbackHttpMethod.Post, param, body, responseContent);
    }

    public void AddWechatAppGetCallbackRecord(string param, string body, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatApp, WechatCallbackHttpMethod.Get, param, body, responseContent);
    }

    public void AddWechatAppPostCallbackRecord(string param, string body, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatApp, WechatCallbackHttpMethod.Post, param, body, responseContent);
    }

    private void AddCallbackRecord(WechatSystemType systemType, WechatCallbackHttpMethod httpMethod, string param, string requestBody, string responseContent) {
        // 没有微信报警的话，一般情况下就禁用这个记录
        if (trackingEnabled) {
            var callback = new WechatCallbackInfo {
                SystemType = systemType,
                Method = httpMethod,
                Params = param,
                RequestBody = requestBody,
                Result = responseContent
            };

            session.Save(callback);
            session.Flush();
        }
    }
}