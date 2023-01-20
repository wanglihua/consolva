using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderExcelController : Controller {
    private readonly ISession session;

    public LogisticsOrderExcelController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/admin/logistics/order/excel/export")]
    public IActionResult Export() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/excel/import")]
    public IActionResult Import() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}