using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppLogin;

public class WechatAppLoginShowLoginModel {
    /// <summary>
    /// 用户服务协议
    /// </summary>
    [JsonProperty("serviceAgreementUrl")]
    public string ServiceAgreementUrl { get; set; } = string.Empty;

    /// <summary>
    /// 隐私说明
    /// </summary>
    [JsonProperty("privacyStatementUrl")]
    public string PrivacyStatementUrl { get; set; } = string.Empty;
}