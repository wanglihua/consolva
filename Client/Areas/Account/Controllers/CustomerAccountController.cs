using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerAccount;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
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
            Data = new CustomerAccountIndexModel {
                
            }
        });
    }
}