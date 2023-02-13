using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppCallbackController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

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
    public IActionResult CallbackPost() {
        foreach (var query in Request.Query) {
            log.Error($"query, key: {query.Key}, value: {string.Join(",", query.Value)}");
        }

        foreach (var form in Request.Form) {
            log.Error($"form, key: {form.Key}, value: {string.Join(",", form.Value)}");
        }

        return Content("");
    }

    [NonAction]
    private string GetParamFormPostModel(PostModel postModel) {
        if (wechatCallbackService.TrackingEnabled == false) {
            return string.Empty;
        }

        return $"DomainId: {postModel.DomainId}\rAppId: {postModel.AppId}\rSignature: {postModel.Signature}\rMsg_Signature: {postModel.Msg_Signature}\rTimestamp: {postModel.Timestamp}\rNonce: {postModel.Nonce}\rToken: {postModel.Token}\rEncodingAESKey: {postModel.EncodingAESKey}";
    }
}