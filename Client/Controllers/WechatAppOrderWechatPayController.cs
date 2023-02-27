using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppOrderWechatPay;
using shunda.Core.Util;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 订单支付
/// </summary>
public class WechatAppOrderWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppOrderWechatPayController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/wechat/app/order/wechat/pay/create")]
    public IActionResult Create([FromBody] WechatAppOrderWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppOrderWechatPayCreateParamValidator, WechatAppOrderWechatPayCreateParam>(param);
        
        

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/wechat/app/order/wechat/pay/notify")]
    public IActionResult Notify() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}