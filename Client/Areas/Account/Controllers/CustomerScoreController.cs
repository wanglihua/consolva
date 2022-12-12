using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreController : Controller
{
    private readonly ISession session;

    public CustomerScoreController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/score")]
    public IActionResult Index()
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}