using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Admin.Areas.Express.Models.ExpressAoPinImport;
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

    [HttpGet]
    [Route("/admin/express/ao/pin/import")]
    public IActionResult Index() {
        return View("ExpressAoPinImportIndex");
    }

    [HttpPost]
    [Route("/admin/express/ao/pin/import/do")]
    public IActionResult Import(IFormFile formFile) {
        var streamReader = new StreamReader(formFile.OpenReadStream());
        var expressList = JsonConvert.DeserializeObject<List<ExpressAoPinImportExpress>>(streamReader.ReadToEnd());

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}