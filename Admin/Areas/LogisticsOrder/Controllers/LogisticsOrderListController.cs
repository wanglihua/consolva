using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderListController : Controller {
    private ISession session;

    public LogisticsOrderListController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/active")]
    public IActionResult IndexActive() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpGet]
    [Route("/admin/logistics/order/complete")]
    public IActionResult IndexComplete() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpGet]
    [Route("/admin/logistics/order/cancel")]
    public IActionResult IndexCancel() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}