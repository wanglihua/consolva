using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using shunda.Domain.Wechat;

namespace shunda.Service.Wechat.CustomMessageHandler;

/// <summary>
/// 自定义MessageHandler
/// </summary>
public partial class CustomMessageHandler {
    /// <summary>
    /// 通过二维码扫描关注扫描事件
    /// </summary>
    /// <param name="requestMessage"></param>
    /// <returns></returns>
    public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage) {
        // to do 好像扫描带参数关注时不走这里 ??
        var openId = requestMessage.FromUserName;

        string accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();
        var userInfo = UserApi.Info(accessToken, openId);
        string nickName = userInfo.nickname;

        int? sceneId = null;
        if (!string.IsNullOrEmpty(requestMessage.EventKey)) {
            sceneId = int.Parse(requestMessage.EventKey.Replace("qrscene_", ""));
        }

        wechatFollowService.NewFellow(openId, nickName, sceneId);

        var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
        responseMessage.Content = wechatOfficialAccountReplyService.GetReply(WechatOfficialAccountReplyType.FollowReply);
        return responseMessage;
    }

    /// <summary>
    /// 订阅（关注）事件
    /// </summary>
    /// <returns></returns>
    public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage) {
        var openId = requestMessage.FromUserName;

        string accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();
        var userInfo = UserApi.Info(accessToken, openId);
        string nickName = userInfo.nickname;

        int? sceneId = null;
        if (!string.IsNullOrEmpty(requestMessage.EventKey)) {
            sceneId = int.Parse(requestMessage.EventKey.Replace("qrscene_", ""));
        }

        wechatFollowService.NewFellow(openId, nickName, sceneId);

        var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
        responseMessage.Content = wechatOfficialAccountReplyService.GetReply(WechatOfficialAccountReplyType.FollowReply);
        return responseMessage;
    }

    /// <summary>
    /// 退订
    /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
    /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
    /// </summary>
    public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage) {
        var openId = requestMessage.FromUserName;
        wechatFollowService.UnFollow(openId);

        return new ResponseMessageNoResponse(); //不返回任何消息
    }
}