using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsTrack.Controllers;

[LogisticsTrackArea]
[NoLoginRequired]
public class SeventeenTrackWebHookController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public SeventeenTrackWebHookController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/admin/seventeen/track/web/hook/notify")]
    public IActionResult Notify() {
        if (Request.Headers.ContainsKey("sign") && Request.Headers["sign"].Count != 0) {
            log.Error("sign:");
            log.Error(Request.Headers["sign"][0]);
        }

        StreamReader streamReader = new StreamReader(Request.Body);
        string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

        log.Error("requestBody:");
        log.Error(requestBody);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}