using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppOrderPay;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

public class WechatAppOrderPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppOrderPayController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/wechat/app/order/pay/create/pay")]
    public IActionResult CreatePay([FromBody] WechatAppOrderPayCreatePayParam param) {
        // todo validator

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}