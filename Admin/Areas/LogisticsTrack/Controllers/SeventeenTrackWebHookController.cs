using System.Net;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Config;
using shunda.Core.Util;
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
        if (Request.Headers.ContainsKey("sign") == false || Request.Headers["sign"].Count == 0) {
            return StatusCode((int)HttpStatusCode.NotAcceptable);
        }

        string sign = Request.Headers["sign"][0];
        log.Error("sign:");
        log.Error(sign);

        StreamReader streamReader = new StreamReader(Request.Body);
        string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

        log.Error("requestBody:");
        log.Error(requestBody);

        // 签名验证
        if (sign != SHA256Util.Encrypt($"{requestBody}/{LogisticsTrackConfig.SeventeenTrackToken}")) {
            return StatusCode((int)HttpStatusCode.NotAcceptable);
        }


        return StatusCode((int)HttpStatusCode.OK);
    }
}