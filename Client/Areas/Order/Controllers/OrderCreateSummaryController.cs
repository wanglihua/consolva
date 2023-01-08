using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateSummaryController : Controller {
    private readonly ISession session;

    public OrderCreateSummaryController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/order/create/summary")]
    public IActionResult Summary() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}