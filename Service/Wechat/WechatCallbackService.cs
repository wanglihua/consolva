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

    public void EnableTracking() {
        trackingEnabled = true;
    }

    public void DisabledTracking() {
        trackingEnabled = false;
    }

    public void AddWechatOfficialAccountGetCallbackRecord(string param, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatOfficialAccount, WechatCallbackHttpMethod.Get, param, string.Empty, responseContent);
    }

    public void AddWechatOfficialAccountPostCallbackRecord(string param, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatOfficialAccount, WechatCallbackHttpMethod.Post, param, string.Empty, responseContent);
    }

    public void AddWechatAppGetCallbackRecord(string param, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatApp, WechatCallbackHttpMethod.Get, param, string.Empty, responseContent);
    }

    public void AddWechatAppPostCallbackRecord(string param, string responseContent) {
        AddCallbackRecord(WechatSystemType.WechatApp, WechatCallbackHttpMethod.Post, param, string.Empty, responseContent);
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