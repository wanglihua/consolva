namespace shunda.Core.Config;

public class SiteConfig {
    public static string SiteIP => "127.0.0.1";
    public static string SiteUrlBase => "https://abc.com";
    public static string StaticFolder => "/Users/xxx/projects/shunda/Admin/wwwroot/static";

    public const string AdminErrorHandlerUrl = "/admin/error";
    public const string JobErrorHandlerUrl = "/job/error";
    public const string ClientErrorHandlerUrl = "/client/error";

    /// <summary>
    /// 是否启用请求日志
    /// </summary>
    public static bool RequestLogEnabled { get; set; } = true;
}