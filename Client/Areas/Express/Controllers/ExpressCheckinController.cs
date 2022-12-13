using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
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

    private IQueryable<ExpressInfo> GetListQuery(ExpressCheckinListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return session.Query<ExpressInfo>()
            .Where(e => e.Customer!.Id == loginCustomerId
                        && (e.Status == ExpressStatus.WaitOrder || e.Status == ExpressStatus.CheckIn));
    }
}