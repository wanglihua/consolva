using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressReturn;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressReturnController : Controller
{
    private readonly ISession session;

    public ExpressReturnController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/express/return/list")]
    public IActionResult List([FromBody] ExpressReturnListParam param)
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}