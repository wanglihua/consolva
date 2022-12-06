using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class TestController : Controller
{
    private readonly ISession session;

    public TestController(ISession session)
    {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/test")]
    public IActionResult Index()
    {
        Thread.Sleep(3000);

        // throw new ValidationException("操作发生错误！");

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}