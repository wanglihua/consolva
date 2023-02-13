using Microsoft.AspNetCore.Mvc;
using NLog;
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
    public IActionResult CallbackGet() {
        foreach (var query in Request.Query) {
            log.Error($"query, key: {query.Key}, value: {string.Join(",", query.Value)}");
        }

        foreach (var form in Request.Form) {
            log.Error($"form, key: {form.Key}, value: {string.Join(",", form.Value)}");
        }

        return Content("");
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
}