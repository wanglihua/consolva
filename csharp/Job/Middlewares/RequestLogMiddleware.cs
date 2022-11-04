using NLog;

namespace shunda.Job.Middlewares;

/// <summary>
/// 这个只用于网站请求时间
/// </summary>
public class RequestLogMiddleware
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly RequestDelegate next;

    public RequestLogMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        DateTime requestBeginTime = DateTime.Now;
        // log.Trace($"Begin >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

        await next.Invoke(context);

        DateTime requestEndTime = DateTime.Now;
        double requestTotalSeconds = (requestEndTime - requestBeginTime).TotalSeconds;

        int longRequestSeconds = 15;

        if (requestTotalSeconds > longRequestSeconds) // 记录大于 15 秒的请求
        {
            log.Error($"检测到请求响应太慢，响应时间：{requestTotalSeconds} 秒");
        }

        // log.Trace($"Request Total Seconds: {requestTotalSeconds:0.000000}{Environment.NewLine}End   <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
    }
}