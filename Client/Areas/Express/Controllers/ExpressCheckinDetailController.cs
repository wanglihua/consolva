using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressCheckinDetail;
using shunda.Client.Services;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinDetailController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinDetailController(ISession session,
     LoginCustomerService loginCustomerService
     )
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/checkin/detail")]
    public IActionResult Detail([FromBody] IdParam param)
    {
        ExpressCheckinDetailModel model = new ExpressCheckinDetailModel
        {
        };

        return Json(new JsonResultData
        {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }
}