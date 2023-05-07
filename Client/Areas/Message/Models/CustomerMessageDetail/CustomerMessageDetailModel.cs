using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageDetail;

public class CustomerMessageDetailModel {
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("customerCode")]
    public string CustomerCode { get; set; } = string.Empty;

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("sendTime")]
    public string SendTime { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("urlTitle")]
    public string UrlTitle { get; set; } = string.Empty;
}