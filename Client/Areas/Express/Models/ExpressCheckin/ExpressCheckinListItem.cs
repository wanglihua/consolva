using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckin;

public class ExpressCheckinListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("expressCompany")]
    public string ExpressCompany { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    /// <summary>
    /// 是不是禁止类
    /// </summary>
    [JsonProperty("forbidden")]
    public bool Forbidden { get; set; }

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    // [JsonProperty("returnPickupCode")]
    // public string ReturnPickupCode { get; set; } = string.Empty;

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    // [JsonProperty("claimTime")]
    // public string ClaimTime { get; set; } = string.Empty;

    // [JsonProperty("returnTrackingNo")]
    // public string ReturnTrackingNo { get; set; } = string.Empty;
    //
    // [JsonProperty("returnExpressCompany")]
    // public string ReturnExpressCompany { get; set; } = string.Empty;

    // 客户端暂时没用起来
    [JsonProperty("customerRemark")]
    public string CustomerRemark { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    // 客户端暂时没用起来
    [JsonProperty("serviceRemark")]
    public string ServiceRemark { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("goodsList")]
    public IList<ExpressCheckinListGoodsListItem> GoodsList = new List<ExpressCheckinListGoodsListItem>();

    [JsonProperty("goodsCount")]
    public int GoodsCount { get; set; }
}