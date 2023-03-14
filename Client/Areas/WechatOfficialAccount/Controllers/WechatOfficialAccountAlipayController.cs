using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.WechatOfficialAccount.Filters;
using shunda.Core.Web;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

/// <summary>
/// 公众号支付宝支付 显示支付订单
/// </summary>
[WechatOfficialAccountArea]
public class WechatOfficialAccountAlipayController : Controller {
    [HttpGet]
    [NoLoginRequired]
    [ServiceFilter(typeof(WechatOfficialAccountAutoLoginFilter))]
    [Route("/client/wechat/official/account/alipay")]
    public IActionResult Index() {
        return View("WechatOfficialAccountAlipayIndex");
    }
}