using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressExpect;
using shunda.Client.Services;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressExpectController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressExpectController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/expect/list")]
    public IActionResult List([FromBody] ExpressExpectListQueryParam queryParam)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var trackingNoQuery = queryParam.TrackingNoQuery;
        
        

        return Json(new {
            success = true,
            message = "操作成功！",
        });
    }
}