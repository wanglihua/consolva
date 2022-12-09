using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressClaim;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

public class ExpressClaimController : Controller
{
    private readonly ISession session;

    public ExpressClaimController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/express/claim/list")]
    public IActionResult List([FromBody] ExpressClaimListParam param)
    {
        // 这里只显示面单客户ID不能解析获得的快递单

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}