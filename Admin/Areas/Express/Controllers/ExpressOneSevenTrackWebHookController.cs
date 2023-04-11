using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[NoLoginRequired]
public class ExpressOneSevenTrackWebHookController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ExpressOneSevenTrackWebHookController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/admin/express/one/seven/track/web/hook/notify")]
    public IActionResult Notify() {
        StreamReader streamReader = new StreamReader(Request.Body);
        string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

        log.Error("requestBody");
        log.Error(requestBody);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}