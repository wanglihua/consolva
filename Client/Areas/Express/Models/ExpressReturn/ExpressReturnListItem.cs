using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("expressCompany")]
    public string ExpressCompany { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    [JsonProperty("returnPickupCode")]
    public string ReturnPickupCode { get; set; } = string.Empty;

    [JsonProperty("addTime")]
    public string AddTime { get; set; } = string.Empty;

    [JsonProperty("claimTime")]
    public string ClaimTime { get; set; } = string.Empty;

    [JsonProperty("returnTrackingNo")]
    public string ReturnTrackingNo { get; set; } = string.Empty;

    [JsonProperty("returnExpressCompany")]
    public string ReturnExpressCompany { get; set; } = string.Empty;

    [JsonProperty("customerRemark")]
    public string CustomerRemark { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    [JsonProperty("serviceRemark")]
    public string ServiceRemark { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("goodsList")]
    public IList<ExpressReturnGoodsListItem> GoodsList { get; set; } = new List<ExpressReturnGoodsListItem>();
}

public class ExpressReturnGoodsListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("goodsName")]
    public string GoodsName { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonProperty("price")]
    public string Price { get; set; } = string.Empty;
}