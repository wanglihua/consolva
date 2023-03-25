using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppLogin; 

public class WechatAppLoginModel {
    [JsonProperty("agreed")]
    public bool Agreed { get; set; }
}