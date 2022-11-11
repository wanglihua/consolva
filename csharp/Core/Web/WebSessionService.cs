using shunda.Core.Service;
using Microsoft.AspNetCore.Http;

namespace shunda.Core.Web;

/// <summary>
/// 自定义的 web session 实现
/// 
/// 保存数据到 "session" 中的时候，注意失效时间的 “滑动”,
/// 就是从 session 取数据的时候，可以再把这个数据重新往 session 里面放一下, 就实现了 “滑动”
/// 主要是 登录用户 id 的信息的滑动，别的 session 信息实际上都只是在 本地内存 中的缓存，缓存中没有了，就数据库中取一下。
///
/// 这个一定要设置成是 Scoped
/// </summary>
[Service(ServiceType.Scoped)]
public class WebSessionService
{
    private static readonly string SESSION_ID_COOKIE_KEY = "shunda_session_id";
    private static readonly string SESSION_ID_SALT = "E9ADB9C0D30208FEA38B359924D5D58D";

    /// <summary>
    /// 这里存储的是跟和客户端传过来的原始 session id 不同的，是进行二次加密过的
    /// </summary>
    public string SessionId { get; private set; } = string.Empty;

    public void SetSessionId(string sessionIdOrigin, HttpRequest request)
    {
        this.SessionId = sessionIdOrigin;
    }

    public string GetSessionIdFromRequest(HttpRequest request)
    {
        string sessionId = request.Cookies[SESSION_ID_COOKIE_KEY] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(sessionId) == false && sessionId.Contains(","))
        {
            // 发现有一个key有两个值的情况

            sessionId = sessionId.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .MaxBy(e => e) ?? string.Empty;
        }

        return sessionId;
    }

    public string SetNewSessionIdToResponse(HttpRequest request, HttpResponse response)
    {
        string sessionId = NewSessionId();
        SetSessionId(sessionId, request);
        PutSessionIdToResponse(request, response, sessionId);

        return sessionId;
    }

    public static string NewSessionId()
    {
        return DateTime.Now.ToString("yyMMddHHmmss") + Guid.NewGuid().ToString("N");
    }

    private static readonly int SESSION_ID_LENGTH = NewSessionId().Length;

    public bool IsValiadSessionId(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return false;
        }

        if (sessionId.Length < SESSION_ID_LENGTH)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 在cookie中，添加新的 session id，同时也清除和失效了原来的 session id
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="sessionId"></param>
    public void PutSessionIdToResponse(HttpRequest request, HttpResponse response, string sessionId)
    {
#if DEBUG
        response.Cookies.Append(SESSION_ID_COOKIE_KEY, sessionId, new CookieOptions() {
            HttpOnly = true,
            IsEssential = true,
            Path = "/",
            Expires = DateTimeOffset.Now.AddDays(100),
            MaxAge = TimeSpan.FromDays(100)
        });

#else
            response.Cookies.Append(SessionIdCookieKey, sessionId, new CookieOptions() {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddHours(1), // 这个时间应该稍微大于 登陆过期时间，就是登录标志的 redis 过期时间
                MaxAge = TimeSpan.FromHours(1)
            });
#endif
    }
}