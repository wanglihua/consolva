using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

public class CustomerAccountController : Controller
{
    private readonly ISession session;

    public CustomerAccountController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/account")]
    public IActionResult Index()
    {
        
        
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}