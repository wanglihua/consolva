namespace shunda.Core.Config;

public class SiteConfig
{
    public const string ErrorHandlerUrl = "/error";

    /// <summary>
    /// 是否启用请求日志
    /// </summary>
    public static bool RequestLogEnabled { get; set; } = true;
}