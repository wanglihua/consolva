using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;

namespace shunda.Client.Middlewares;

/// <summary>
/// 这个 middleware 的作用是保持 cookie 中一直有一个 session id，
/// 保持 WebSessionService的成员SessionId中的值 和 cookie 中的 session id 一致
/// </summary>
public class SessionIdMiddleware
{
    private readonly RequestDelegate next;

    public SessionIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context, [FromServices] WebSessionService webSessionService)
    {
        var sessionId = webSessionService.GetSessionIdFromRequest(context.Request);

        if (webSessionService.IsValiadSessionId(sessionId) == false) sessionId = WebSessionService.NewSessionId();

        webSessionService.PutSessionIdToResponse(context.Request, context.Response, sessionId);

        webSessionService.SetSessionId(sessionId, context.Request); // 下面的逻辑中已经在使用这个 WebSessionService.SessionId，所以应该放在下面的逻辑之前

        await next.Invoke(context);
    }
}