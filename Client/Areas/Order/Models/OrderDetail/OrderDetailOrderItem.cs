using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailOrderItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonProperty("orderStatus")]
    public string OrderStatus { get; set; } = string.Empty;

    [JsonProperty("hasPacked")]
    public bool HasPacked { get; set; }

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("orderAmount")]
    public string OrderAmount { get; set; } = string.Empty;

    [JsonProperty("arearsAmount")]
    public string ArearsAmount { get; set; } = string.Empty;

    #region checkin size weight data

    [JsonProperty("hasCheckinSizeWeightData")]
    public bool HasCheckinSizeWeightData { get; set; }

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

    [JsonProperty("firstModeType")]
    public string FirstModeType { get; set; } = string.Empty;

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

    [JsonProperty("insurance")]
    public string Insurance { get; set; } = string.Empty;

    [JsonProperty("gst")]
    public string Gst { get; set; } = string.Empty;

    [JsonProperty("repackType")]
    public string RepackType { get; set; } = string.Empty;

    [JsonProperty("customerRemark")]
    public string CustomerRemark { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    [JsonProperty("serviceRemark")]
    public string ServiceRemark { get; set; } = string.Empty;
}