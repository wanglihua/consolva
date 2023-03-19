using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using shunda.Admin.Areas.Fee.Models.PaypalWebhookAdmin;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Service.Pay;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(AdminOnly = true)]
public class PaypalWebhookAdminController : Controller {
    [HttpGet]
    [Route("/admin/paypal/webhook/admin")]
    public IActionResult Index() {
        var model = new PaypalWebhookAdminIndexModel();
        return View("PaypalWebhookAdminIndex", model);
    }

    [HttpPost]
    [Route("/admin/paypal/webhook/admin/create")]
    public IActionResult Create() {
        var apiContext = PaypalUtil.GetApiContext();
        var eventTypeList = WebhookEventType.AvailableEventTypes(apiContext);

        var webhook = Webhook.Create(apiContext, new Webhook() {
            url = SiteConfig.SiteUrlBase + "/client/order/paypal/webhook",
            event_types = eventTypeList.event_types,
        });

        return Json(new JsonResultData {
            Success = true,
            Message = $"操作成功, {webhook.id}",
        });
    }

    [HttpPost]
    [Route("/admin/paypal/webhook/admin/delete")]
    public IActionResult Delete() {
        var apiContext = PaypalUtil.GetApiContext();
        var webhookList = Webhook.GetAll(apiContext);

        foreach (var webhook in webhookList.webhooks) {
            Webhook.Delete(apiContext, webhook.id);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpGet]
    [Route("/admin/paypal/webhook/admin/list")]
    public IActionResult List() {
        var apiContext = PaypalUtil.GetApiContext();
        var webhookList = Webhook.GetAll(apiContext);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = webhookList,
        });
    }
}