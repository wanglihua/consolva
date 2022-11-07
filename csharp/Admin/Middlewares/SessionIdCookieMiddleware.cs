using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;

namespace shunda.Admin.Middlewares;

/// <summary>
///     这个 middleware 的作用是保持 cookie 中一直有一个 session id，
///     保持 WebSessionService的成员SessionId中的值 和 cookie 中的 session id 一致
/// </summary>
public class SessionIdCookieMiddleware
{
    private readonly RequestDelegate next;

    public SessionIdCookieMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, [FromServices] WebSessionService webSessionService)
    {
        var sessionId = webSessionService.GetSessionIdFromRequest(context.Request);

        if (webSessionService.IsValiadSessionId(sessionId) == false) sessionId = WebSessionService.NewSessionId();

        webSessionService.SetSessionIdToResponse(context.Request, context.Response, sessionId);

        webSessionService.SessionId = sessionId; // 下面的逻辑中已经在使用这个 WebSessionService.SessionId，所以应该放在下面的逻辑之前

        await next.Invoke(context);
    }
}