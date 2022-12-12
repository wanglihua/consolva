using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinController : Controller
{
    private ISession session;

    public ExpressCheckinController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/express/checkin/list")]
    public IActionResult List([FromBody] ExpressCheckinListParam param)
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}