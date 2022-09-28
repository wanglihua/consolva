using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using shunda.Service;

namespace shunda.Admin.Filters;

/// <summary>
/// 这个只用于记录网站请求记录
/// </summary>
public class RequestLogActionFilter : IActionFilter
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private SiteRequestLogService siteRequestLogService;

    public RequestLogActionFilter(SiteRequestLogService siteRequestLogService)
    {
        this.siteRequestLogService = siteRequestLogService;
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
            DateTime requestBeginTime = (DateTime)(context.HttpContext.Items["request_begin_time"] ?? DateTime.Now);
            DateTime requestEndTime = DateTime.Now;

            double requestTotalSeconds = (requestEndTime - requestBeginTime).TotalSeconds;

            // 网站请求数据库日志
            try
            {
                var request = context.HttpContext.Request;

                string relativeUrl = request.GetEncodedPathAndQuery();
                if (relativeUrl != "/"
                    && relativeUrl.StartsWith("/admin") == false)
                {
                    log.Error($"url 没有以 /admin 开头: {relativeUrl}");
                }

                // siteRequestLogService.NewSiteRequest(request, (decimal)requestTotalSeconds, useInnerTransaction: false);
            }
            catch (Exception exp) // 不能因为这个功能而影响了全局
            {
                log.Error(exp);
            }
        }
    }
}