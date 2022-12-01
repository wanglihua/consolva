using Newtonsoft.Json;

namespace shunda.Service.Wechat;

public class WechatJsonResult
{
    [JsonProperty("errcode")]
    public int ErrorCode { get; set; }

    [JsonProperty("errmsg")]
    public string ErrorMessage { get; set; } = string.Empty;
}