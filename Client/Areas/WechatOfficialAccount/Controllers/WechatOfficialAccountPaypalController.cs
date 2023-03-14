using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.WechatOfficialAccount.Filters;
using shunda.Core.Web;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

/// <summary>
/// 公众号 PayPal支付 显示支付订单
/// </summary>
[WechatOfficialAccountArea]
public class WechatOfficialAccountPaypalController : Controller {
    [HttpGet]
    [ServiceFilter(typeof(WechatOfficialAccountAutoLoginFilter))]
    [Route("/client/wechat/official/account/paypal")]
    public IActionResult Index() {
        return View("WechatOfficialAccountPaypalIndex");
    }
}