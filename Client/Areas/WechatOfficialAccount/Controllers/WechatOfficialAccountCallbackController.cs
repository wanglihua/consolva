using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.NeuChar.Context;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Service.Wechat;
using shunda.Service.Wechat.CustomMessageHandler;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

#pragma warning disable 0618

[NoLoginRequired]
[WechatOfficialAccountArea]
public class WechatOfficialAccountCallbackController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatCallbackService wechatCallbackService;

    public WechatOfficialAccountCallbackController(ISession session, WechatCallbackService wechatCallbackService) {
        this.session = session;
        this.wechatCallbackService = wechatCallbackService;
    }

    [HttpPost]
    [Route("/client/wechat/official/account/callback/tracking/enable")]
    public IActionResult EnableCallbackTracking() {
        wechatCallbackService.TrackingEnabled = true;

        return Json(new JsonResultData {
            Success = true,
            Message = "回调跟踪启用成功！",
        });
    }

    [HttpPost]
    [Route("/client/wechat/official/account/callback/tracking/disable")]
    public IActionResult DisableCallbackTracking() {
        wechatCallbackService.TrackingEnabled = false;

        return Json(new JsonResultData {
            Success = true,
            Message = "回调跟踪禁用成功！",
        });
    }

    private const string callbackUrl = "/client/wechat/official/account/callback";

    [HttpGet]
    [Route(callbackUrl)]
    public IActionResult CallbackGet(PostModel postModel, string echostr) {
        string responseContent;

        string token = WechatOfficialAccountConfig.Token;

        if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token)) {
            responseContent = echostr; //返回随机字符串则表示验证通过
        } else {
            responseContent = $"failed:{postModel.Signature}, {CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token)}。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。";
        }

        wechatCallbackService.AddWechatOfficialAccountGetCallbackRecord(GetParamFormPostModel(postModel), string.Empty, responseContent);

        return Content(responseContent);
    }

    [HttpPost]
    [Route(callbackUrl)]
    [Transactional]
    public IActionResult CallbackPost(PostModel postModel) {
        try {
            // postModel 里面的这三个值都为空
            string token = WechatOfficialAccountConfig.Token;
            string encodingAesKey = WechatOfficialAccountConfig.EncodingAESKey;
            string appId = WechatOfficialAccountConfig.AppId;

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token)) {
                log.Error($"CallbackPost 微信调用参数错误：Signature: {postModel.Signature}, Timestamp: {postModel.Timestamp}, Nonce: {postModel.Nonce}, token: {token}");

                string result = "参数错误！";

                wechatCallbackService.AddWechatOfficialAccountPostCallbackRecord(GetParamFormPostModel(postModel), "body-01", result);

                return Content(result);
            }

            #region 打包 PostModel 信息

            postModel.Token = token;
            postModel.EncodingAESKey = encodingAesKey;
            postModel.AppId = appId;

            #endregion

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。

            // XDocument requestXmlDoc = XDocument.Load(Request.Body); 这个同步读取会报错

            // 这个异步读取就不会报错
            StreamReader streamReader = new StreamReader(Request.Body);
            string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
            XDocument requestXmlDoc = XDocument.Parse(requestBody);

            MessageContextGlobalConfig.UseMessageContext = false;
            var messageHandler = new WechatOfficialAccountCustomMessageHandler(session, requestXmlDoc, postModel);

            // 执行微信处理过程（关键）
            messageHandler.Execute();

            string responseXml = messageHandler.FinalResponseDocument?.ToString() ?? "";

            // 记录微信调用记录
            if (string.IsNullOrWhiteSpace(responseXml) == false) {
                wechatCallbackService.AddWechatOfficialAccountPostCallbackRecord(GetParamFormPostModel(postModel), requestBody, responseXml);
            } else {
                wechatCallbackService.AddWechatOfficialAccountPostCallbackRecord(GetParamFormPostModel(postModel), requestBody, "empty-01");
            }

            if (string.IsNullOrWhiteSpace(responseXml)) {
                responseXml = "error"; // 给个信息否则微信服务器认为没反应
            }

            return Content(responseXml, "text/xml", Encoding.UTF8);
        }
        catch (Exception exp) {
            log.Error("微信公众号CallbackPost发生错误");
            log.Error(exp);

            // 记录微信调用记录
            wechatCallbackService.AddWechatOfficialAccountPostCallbackRecord(GetParamFormPostModel(postModel), "body-02", "empty-02");

            return Content("exception", "text/xml", Encoding.UTF8); // 给个信息否则微信服务器认为没反应
        }
    }

    [NonAction]
    private string GetParamFormPostModel(PostModel postModel) {
        if (wechatCallbackService.TrackingEnabled == false) {
            return string.Empty;
        }

        return $"DomainId: {postModel.DomainId}\rAppId: {postModel.AppId}\rSignature: {postModel.Signature}\rMsg_Signature: {postModel.Msg_Signature}\rTimestamp: {postModel.Timestamp}\rNonce: {postModel.Nonce}\rToken: {postModel.Token}\rEncodingAESKey: {postModel.EncodingAESKey}";
    }
}