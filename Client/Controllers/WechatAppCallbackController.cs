using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.NeuChar.Context;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Service.Wechat;
using shunda.Service.Wechat.CustomMessageHandler;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

#pragma warning disable 0618

[NoLoginRequired]
public class WechatAppCallbackController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatCallbackService wechatCallbackService;

    public WechatAppCallbackController(ISession session,
        WechatCallbackService wechatCallbackService
    ) {
        this.session = session;
        this.wechatCallbackService = wechatCallbackService;
    }

    [HttpPost]
    [Route("/client/wechat/app/callback/tracking/enable")]
    public IActionResult EnableCallbackTracking() {
        wechatCallbackService.TrackingEnabled = true;

        return Json(new JsonResultData {
            Success = true,
            Message = "回调跟踪启用成功！",
        });
    }

    [HttpPost]
    [Route("/client/wechat/app/callback/tracking/disable")]
    public IActionResult DisableCallbackTracking() {
        wechatCallbackService.TrackingEnabled = false;

        return Json(new JsonResultData {
            Success = true,
            Message = "回调跟踪禁用成功！",
        });
    }

    private const string callbackUrl = "/client/wechat/app/callback";

    [HttpGet]
    [Route(callbackUrl)]
    public IActionResult CallbackGet(PostModel postModel, string echostr) {
        string responseContent;

        string token = WechatAppConfig.Token;

        if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token)) {
            responseContent = echostr; //返回随机字符串则表示验证通过
        } else {
            responseContent = $"failed:{postModel.Signature}, {CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token)}。如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。";
        }

        wechatCallbackService.AddWechatAppGetCallbackRecord(GetParamFormPostModel(postModel), string.Empty, responseContent);

        return Content(responseContent);
    }

    [HttpPost]
    [Route(callbackUrl)]
    public IActionResult CallbackPost(PostModel postModel) {
        #region 记录微信传过来的参数用于开始时的调试

        foreach (var query in Request.Query) {
            log.Error($"query, key: {query.Key}, value: {string.Join(",", query.Value)}");
        }

        /*
        foreach (var form in Request.Form) {
            log.Error($"form, key: {form.Key}, value: {string.Join(",", form.Value)}");
        }
        */

        StreamReader streamReader = new StreamReader(Request.Body);
        string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
        log.Error($"request body: {requestBody}");

        #endregion

        try {
            // postModel 里面的这三个值都为空
            string token = WechatAppConfig.Token;
            string encodingAesKey = WechatAppConfig.EncodingAESKey;
            string appId = WechatAppConfig.AppId;

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token)) {
                log.Error($"CallbackPost 微信调用参数错误：Signature: {postModel.Signature}, Timestamp: {postModel.Timestamp}, Nonce: {postModel.Nonce}, token: {token}");

                string result = "参数错误！";

                wechatCallbackService.AddWechatAppPostCallbackRecord(GetParamFormPostModel(postModel), "body-01", result);

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
            // StreamReader streamReader = new StreamReader(Request.Body); 改在上面记录日志时调用了
            // string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
            XDocument requestXmlDoc = XDocument.Parse(requestBody);

            MessageContextGlobalConfig.UseMessageContext = false;
            var messageHandler = new WechatAppCustomMessageHandler(session, requestXmlDoc, postModel);

            // 执行微信处理过程（关键）
            messageHandler.Execute();

            string responseXml = messageHandler.FinalResponseDocument?.ToString() ?? "";

            // 记录微信调用记录
            if (string.IsNullOrWhiteSpace(responseXml) == false) {
                wechatCallbackService.AddWechatAppPostCallbackRecord(GetParamFormPostModel(postModel), requestBody, responseXml);
            } else {
                wechatCallbackService.AddWechatAppPostCallbackRecord(GetParamFormPostModel(postModel), requestBody, "empty-01");
            }

            if (string.IsNullOrWhiteSpace(responseXml)) {
                responseXml = "error"; // 给个信息否则微信服务器认为没反应
            }

            return Content(responseXml, "text/xml", Encoding.UTF8);
        }
        catch (Exception exp) {
            log.Error("微信小程序CallbackPost发生错误");
            log.Error(exp);

            // 记录微信调用记录
            wechatCallbackService.AddWechatAppPostCallbackRecord(GetParamFormPostModel(postModel), "body-02", "empty-02");

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