using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailController : Controller {
    private readonly ISession session;

    public LogisticsOrderDetailController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/detail/{id:int}")]
    public IActionResult Index(int id) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}