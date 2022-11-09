namespace shunda.Core.Config;

public class SiteConfig
{
    public static string SiteRoot => "http://localhost:9001";
    
    public const string AdminErrorHandlerUrl = "/admin/error";

    public const string JobErrorHandlerUrl = "/job/error";

    public const string ClientErrorHandlerUrl = "/client/error";

    /// <summary>
    /// 是否启用请求日志
    /// </summary>
    public static bool RequestLogEnabled { get; set; } = true;
}