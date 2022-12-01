using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace shunda.Admin.Filters;

public class HeaderActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("HeaderActionFilter OnActionExecuting");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("HeaderActionFilter OnActionExecuted");

        var headers = context.HttpContext.Response.Headers;

        headers["Dev-Company"] = "Jiaxing Huadu Information Technolony Co.,Ltd";
        headers["Main-Developer"] = "wanglihua";
    }
}