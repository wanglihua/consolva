using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressAoPinImportController : Controller {
    private readonly ISession session;

    public ExpressAoPinImportController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/admin/express/ao/pin/import")]
    public IActionResult Index() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/admin/express/ao/pin/import/do")]
    public IActionResult Import(IFormFile formFile) {
        
        
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}