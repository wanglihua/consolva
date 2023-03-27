using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;

namespace shunda.Admin.Areas.App.Controllers;

[AdminAuth]
[AppArea]
public class KeepLoginController : Controller {
    [HttpPost]
    [Route("/admin/app/keep/login/check")]
    public IActionResult Check() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}