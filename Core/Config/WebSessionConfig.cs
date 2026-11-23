namespace shunda.Core.Config;

/// <summary>
/// session id key
/// 后期可以考虑后台管理和客户端使用不同的 session id key
/// </summary>
public class WebSessionConfig {
    public static string SessionIdCookieKey => "shunda_session_id";
    public static string SessionIdHeaderKey => "session-id";
    public static TimeSpan LOGIN_USER_SESSION_TIMEOUT => TimeSpan.FromMinutes(60);
    public static TimeSpan LOGIN_CUSTOMER_SESSION_TIMEOUT => TimeSpan.FromMinutes(120); // 120 分钟已经足够长
}