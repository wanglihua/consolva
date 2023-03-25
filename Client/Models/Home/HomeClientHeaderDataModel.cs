using Newtonsoft.Json;

namespace shunda.Client.Models.Home; 

public class HomeClientHeaderDataModel {
    /// <summary>
    /// 这里 customer code 带有 超级VIP，VIP等的信息，用户页面头部显示
    /// </summary>
    [JsonProperty("customerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [JsonProperty("agreed")]
    public bool Agreed { get; set; }
}