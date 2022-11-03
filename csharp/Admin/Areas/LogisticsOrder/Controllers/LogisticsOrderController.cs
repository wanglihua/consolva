using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
// [AdminAuth(PassRes.LogisticsOrder.)] todo 补上
public class LogisticsOrderController : Controller
{
    private ISession session;

    public LogisticsOrderController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/logistics/order/detail/{id:int}")]
    public IActionResult Detail(int id)
    {
        return Json(true);
    }
}