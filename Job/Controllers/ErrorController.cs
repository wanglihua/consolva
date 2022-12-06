using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Config;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Job.Models.Error;

namespace shunda.Job.Controllers;

[NoLoginRequired]
public class ErrorController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(SiteConfig.ClientErrorHandlerUrl)]
    public IActionResult Error()
    {
        IExceptionHandlerFeature feature = HttpContext.Features.Get<IExceptionHandlerFeature>() ?? new ExceptionHandlerFeature();
        Exception exp = feature?.Error ?? new Exception();

        log.Error(exp);

        if (WebUtil.ShouldShowJson(Request))
        {
            return Json(new JsonResultData {
                Success = false,
                Message = exp.Message,
            });
        }

        ErrorViewModel model = new ErrorViewModel();

        model.Message = exp.Message;
        model.StackTrace = exp.StackTrace ?? string.Empty;

        return View("Error", model);
    }
}