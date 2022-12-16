using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Message.Controllers;

public class CustomerMessageController : Controller
{
    private readonly ISession session;

    public CustomerMessageController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/message/list")]
    public IActionResult List()
    {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}