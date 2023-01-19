using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
public class HomeController : Controller {
    private readonly ISession session;

    public HomeController(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 初始化 web session
    /// 在 app 启动时调用这个接口一次
    /// </summary>
    [HttpPost]
    [Route("/admin/app/home/init/session")]
    [NoLoginRequired]
    public IActionResult InitSession() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}