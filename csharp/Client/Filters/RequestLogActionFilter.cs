using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace shunda.Client.Filters;

public class RequestLogActionFilter : IActionFilter
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public RequestLogActionFilter()
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // log.Trace("RequestLogActionFilter OnActionExecuting");

        context.HttpContext.Items["request_begin_time"] = DateTime.Now;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // log.Trace("RequestLogActionFilter OnActionExecuted");

        if (context.HttpContext.Items["request_begin_time"] != null)
        {
            var requestBeginTime = (DateTime)(context.HttpContext.Items["request_begin_time"] ?? DateTime.Now);
            var requestEndTime = DateTime.Now;

            var requestTotalSeconds = (requestEndTime - requestBeginTime).TotalSeconds;

            // 网站请求数据库日志
            try
            {
                var request = context.HttpContext.Request;

                var relativeUrl = request.GetEncodedPathAndQuery();
                if (relativeUrl != "/"
                    && relativeUrl != "/favicon.ico"
                    && relativeUrl.StartsWith("/client") == false)
                    log.Error($"url 没有以 /client 开头: {relativeUrl}");
            }
            catch (Exception exp) // 不能因为这个功能而影响了全局
            {
                log.Error(exp);
            }
        }
    }
}