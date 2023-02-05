using Newtonsoft.Json;

namespace shunda.Service.LogisticsOrder; 

public class LogisticsOrderClientListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("orderStatus")]
    public string OrderStatus { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("arearsAmount")]
    public string ArearsAmount { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    [JsonProperty("totalVolume")]
    public string TotalVolume { get; set; } = string.Empty;

    [JsonProperty("totalVolumeWeight")]
    public string TotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("totalActualWeight")]
    public string TotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("maxSideLength")]
    public string MaxSideLength { get; set; } = string.Empty;

    [JsonProperty("firstModeName")]
    public string FirstModeName { get; set; } = string.Empty;

    [JsonProperty("createTime")]
    public string CreateTime { get; set; } = string.Empty;

    [JsonProperty("packTime")]
    public string PackTime { get; set; } = string.Empty;

    [JsonProperty("transferTime")]
    public string TransferTime { get; set; } = string.Empty;

    [JsonProperty("deliverTime")]
    public string DeliverTime { get; set; } = string.Empty;

    [JsonProperty("shouldPay")]
    public bool ShouldPay { get; set; }

    [JsonProperty("paid")]
    public bool Paid { get; set; }

    [JsonProperty("canCancel")]
    public bool CanCancel { get; set; }

    [JsonProperty("airExpressTrackingNo")]
    public string AirExpressTrackingNo { get; set; } = string.Empty;
}