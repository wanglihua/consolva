using System.Xml.Linq;
using NHibernate;
using NLog;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Tencent;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Customer;

namespace shunda.Service.Wechat.CustomMessageHandler;

/// <summary>
/// 自定义MessageHandler
/// 把MessageHandler作为基类，重写对应请求的处理方法
/// 每个回调生成这个类的一个实例
/// </summary>
public partial class CustomMessageHandler : MessageHandler<CustomMessageContext> {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    private readonly WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService;
    private readonly WechatOfficialAccountReplyService wechatOfficialAccountReplyService;
    private readonly WechatOfficialAccountReplyKeywordService wechatOfficialAccountReplyKeywordService;
    private readonly WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService;

    /// <summary>
    /// 模板消息集合（Key：checkCode，Value：OpenId）
    /// </summary>
    public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

    public CustomMessageHandler(ISession session, XDocument document, PostModel postModel, int maxRecordCount = 0)
        : base(document, postModel, maxRecordCount) {
        this.session = session;
        this.wechatOfficialAccountFollowerService = new WechatOfficialAccountFollowerService(session, new CustomerPromotionService(session));
        this.wechatOfficialAccountReplyService = new WechatOfficialAccountReplyService(session);
        this.wechatOfficialAccountReplyKeywordService = new WechatOfficialAccountReplyKeywordService(session);
        this.wechatOfficialAccountAccessTokenService = new WechatOfficialAccountAccessTokenService(session);

        this.OmitRepeatedMessage = true;
    }

    /// <summary>
    /// 初始化，获取RequestDocument。（必须要完成 RequestMessage 数据赋值）。
    /// Init中需要对上下文添加当前消息（如果使用上下文）；以及判断消息的加密情况，对解密信息进行解密
    /// </summary>
    public override XDocument? Init(XDocument postDataDocument, IEncryptPostModel postModel) {
        //进行加密判断并处理
        PostModel = postModel as PostModel;
        var postDataStr = postDataDocument.ToString();

        XDocument decryptDoc = postDataDocument;

        if (PostModel != null
            && !PostModel.Token.IsNullOrWhiteSpace()
            && postDataDocument.Root?.Element("Encrypt") != null
            && !string.IsNullOrEmpty(postDataDocument.Root?.Element("Encrypt")?.Value)) {
            //使用了加密
            EcryptRequestDocument = postDataDocument;

            WXBizMsgCrypt msgCrype = new WXBizMsgCrypt(PostModel.Token, PostModel.EncodingAESKey, ((PostModel)PostModel).AppId);
            string msgXml = string.Empty;
            var result = msgCrype.DecryptMsg(PostModel.Msg_Signature, PostModel.Timestamp, PostModel.Nonce, postDataStr, ref msgXml);

            //判断result类型
            if (result != 0) {
                return null;
            }

            decryptDoc = XDocument.Parse(msgXml); //完成解密
        }

        RequestMessage = GetRequestEntity(new CustomMessageContext(), decryptDoc);
        return decryptDoc;
        //消息上下文记录将在 base.CommonInitialize() 中根据去重等条件判断后进行添加
    }

    private IRequestMessageBase GetRequestEntity(CustomMessageContext messageContext, XDocument doc) {
        try {
            RequestMsgType msgType = MsgTypeHelper.GetRequestMsgType(doc);
            RequestMessageBase requestMessage = (messageContext.GetRequestEntityMappingResult(msgType, doc) as RequestMessageBase)!; // 这一行跟 RequestMessageFactory.GetRequestEntity 里面的不同
            requestMessage.FillEntityWithXml(doc); // EntityHelper.FillEntityWithXml(requestMessage, doc);
            return requestMessage;
        }
        catch (ArgumentException ex) {
            throw new WeixinException(string.Format("RequestMessage转换出错！可能是MsgType不存在！，XML：{0}", doc.ToString()), ex);
        }
    }

    private void SaveMessageRequest(string openId, string messageContent) {
        var customer = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId)
            .Take(1)
            .SingleOrDefault();

        WechatOfficialAccountMessageInfo message = new WechatOfficialAccountMessageInfo {
            Direction = WechatOfficialAccountMessageDirection.CustomerToUser,
            OpenId = openId,
            Customer = customer,
            Message = messageContent,
            Status = WechatOfficialAccountMessageStatus.NotReply,
        };

        session.Save(message);
        session.Flush();
    }

    /// <summary>
    /// 处理文字请求
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <returns></returns>
    public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage) {
        string messageContent = requestMessage.Content;

        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, messageContent);

        // 自动回复
        string responseMessageText = wechatOfficialAccountReplyKeywordService.GetKeywordReply(messageContent);

        if (string.IsNullOrWhiteSpace(responseMessageText)) {
            responseMessageText = wechatOfficialAccountReplyService.GetReply(WechatOfficialAccountReplyType.TextMessageReply);
        }

        ResponseMessageText responseMessage = CreateResponseMessage<ResponseMessageText>();
        responseMessage.Content = responseMessageText;
        return responseMessage;
    }

    public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个短视频信息");
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理图片请求
    /// </summary>
    public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个图片信息");
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理语音请求
    /// </summary>
    public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个语音信息");
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理视频请求
    /// </summary>
    public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个视频信息");
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理链接消息请求
    /// </summary>
    public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个链接信息");
        return new ResponseMessageNoResponse();
    }

    public override IResponseMessageBase OnFileRequest(RequestMessageFile requestMessage) {
        string openId = requestMessage.FromUserName;
        SaveMessageRequest(openId, "你有一个文件信息");
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
    /// </summary>
    public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage) {
        var eventResponseMessage = base.OnEventRequest(requestMessage); //对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
        return eventResponseMessage;
    }

    public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage) {
        // 默认的就都是空消息
        return new ResponseMessageNoResponse();
    }

    public override IResponseMessageBase OnUnknownTypeRequest(RequestMessageUnknownType requestMessage) {
        log.Error(requestMessage.RequestDocument.ToString());
        return new ResponseMessageNoResponse();
    }
}