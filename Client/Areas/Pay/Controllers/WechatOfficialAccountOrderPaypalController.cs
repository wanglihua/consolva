using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号 PayPal支付 处理支付接口
/// </summary>
[NoLoginRequired]
[PayArea]
public class WechatOfficialAccountPaypalController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    [NoLoginRequired]
    [HttpPost]
    [Route("/client/order/paypal/webhook")]
    public IActionResult Webhook() {
        try {
            StringBuilder headerStringBuilder = new StringBuilder();
            var headers = Request.Headers;
            var headerKeys = headers.Keys;
            foreach (var headerKey in headerKeys) {
                headerStringBuilder.Append($"{headerKey}:{headers[headerKey]}{Environment.NewLine}");
            }

            log.Error("paypal webhook header:");
            log.Error(headerStringBuilder.ToString());

            StreamReader streamReader = new StreamReader(Request.Body);
            string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

            log.Error("paypal webhook request body:");
            log.Error(requestBody);
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        // 返回时，起作用的只是 http code 200 
        return Content("success");
    }
}