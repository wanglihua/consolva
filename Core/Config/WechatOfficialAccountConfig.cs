namespace shunda.Core.Config;

public class WechatOfficialAccountConfig {
    public static string AppId => "AppId";
    public static string AppSecret => "AppSecret";
    public static string Token => "Token";
    public static string EncodingAESKey => "EncodingAESKey";

    public static bool LogOAuthAccessToken => false;

    public static readonly string OrderNoticeTemplateId = "OrderNoticeTemplateId";
}