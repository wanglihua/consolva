using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Misc.Controllers;

[MiscArea]
public class NoticeController : Controller {
    private ISession session;

    public NoticeController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/notice/list")]
    public IActionResult List() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}