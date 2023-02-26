using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;

namespace shunda.Service.Wechat.CustomMessageHandler;

#pragma warning disable 0618

/// <summary>
/// 自定义MessageHandler
/// </summary>
public partial class WechatAppCustomMessageHandler {
    public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage) {
        log.Error("OnEvent_ScanRequest:");
        log.Error($"event key: {requestMessage.EventKey}, event name: {requestMessage.EventName}");
        return new ResponseMessageNoResponse();
    }
}