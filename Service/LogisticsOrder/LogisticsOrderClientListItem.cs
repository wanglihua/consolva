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

    // todo packed data

    #region checkin size weight data

    [JsonProperty("checkinTotalPackageCount")]
    public string CheckinTotalPackageCount { get; set; } = string.Empty;

    [JsonProperty("checkinTotalVolume")]
    public string CheckinTotalVolume { get; set; } = string.Empty;

    [JsonProperty("checkinTotalVolumeWeight")]
    public string CheckinTotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("checkinTotalActualWeight")]
    public string CheckinTotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("checkinMaxSideLength")]
    public string CheckinMaxSideLength { get; set; } = string.Empty;

    #endregion

    #region packed size weight data

    [JsonProperty("packedTotalPackageCount")]
    public string PackedTotalPackageCount { get; set; } = string.Empty;

    [JsonProperty("packedTotalVolume")]
    public string PackedTotalVolume { get; set; } = string.Empty;

    [JsonProperty("packedTotalVolumeWeight")]
    public string PackedTotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("packedTotalActualWeight")]
    public string PackedTotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("packedMaxSideLength")]
    public string PackedMaxSideLength { get; set; } = string.Empty;

    #endregion

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