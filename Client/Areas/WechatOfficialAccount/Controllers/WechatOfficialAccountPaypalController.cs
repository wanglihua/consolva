using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;

namespace shunda.Client.Areas.WechatOfficialAccount.Controllers;

/// <summary>
/// 公众号 PayPal支付 显示支付订单
/// </summary>
[WechatOfficialAccountArea]
public class WechatOfficialAccountPaypalController : Controller {
    [HttpPost]
    [Route("/client/wechat/official/account/paypal")]
    public IActionResult Index() {
        // 应该是展现一个订单列表界面
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}