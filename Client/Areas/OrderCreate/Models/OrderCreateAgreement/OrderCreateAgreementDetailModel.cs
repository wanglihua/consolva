using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateAgreement;

public class OrderCreateAgreementDetailModel {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = String.Empty;
}