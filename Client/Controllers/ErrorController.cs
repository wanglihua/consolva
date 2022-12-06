using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Error;
using shunda.Core.Config;
using shunda.Core.Util;
using shunda.Core.Web;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class ErrorController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(SiteConfig.ClientErrorHandlerUrl)]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>() ?? new ExceptionHandlerFeature();
        var exp = feature.Error ?? new Exception();

        log.Error(exp);

        if (WebUtil.ShouldShowJson(Request))
            return Json(new JsonResultData {
                Success = false,
                Message = exp.Message
            });

        var model = new ErrorViewModel();

        model.Message = exp.Message;
        model.StackTrace = exp.StackTrace ?? string.Empty;

        return View("Error", model);
    }
}