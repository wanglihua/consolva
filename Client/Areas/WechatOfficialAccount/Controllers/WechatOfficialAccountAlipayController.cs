using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.WechatOfficialAccount.Filters;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

/// <summary>
/// 公众号支付宝支付 显示支付订单
/// </summary>
[WechatOfficialAccountArea]
public class WechatOfficialAccountAlipayController : Controller {
    [HttpPost]
    [ServiceFilter(typeof(WechatOfficialAccountAutoLoginFilter))]
    [Route("/client/wechat/official/account/alipay")]
    public IActionResult Index() {
        return View("WechatOfficialAccountAlipayIndex");
    }
}