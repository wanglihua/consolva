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

namespace shunda.Service.Wechat.CustomMessageHandler;

/// <summary>
/// 自定义MessageHandler
/// 把MessageHandler作为基类，重写对应请求的处理方法
/// 每个回调生成这个类的一个实例
/// </summary>
public partial class WechatAppCustomMessageHandler : MessageHandler<CustomMessageContext> {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    /// <summary>
    /// 模板消息集合（Key：checkCode，Value：OpenId）
    /// </summary>
    public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

    public WechatAppCustomMessageHandler(ISession session, XDocument document, PostModel postModel, int maxRecordCount = 0)
        : base(document, postModel, maxRecordCount) {
        this.session = session;

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

            // 用于调试
            log.Error("msgXml:");
            log.Error(msgXml);

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

    /// <summary>
    /// 处理文字请求
    /// </summary>
    /// <param name="requestMessage">请求消息</param>
    /// <returns></returns>
    public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage) {
        return new ResponseMessageNoResponse();
    }

    public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage) {
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理图片请求
    /// </summary>
    public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage) {
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理语音请求
    /// </summary>
    public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage) {
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理视频请求
    /// </summary>
    public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage) {
        return new ResponseMessageNoResponse();
    }

    /// <summary>
    /// 处理链接消息请求
    /// </summary>
    public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage) {
        return new ResponseMessageNoResponse();
    }

    public override IResponseMessageBase OnFileRequest(RequestMessageFile requestMessage) {
        return new ResponseMessageNoResponse();
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