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

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    [JsonProperty("returnTrackingNo")]
    public string ReturnTrackingNo { get; set; } = string.Empty;

    [JsonProperty("returnExpressCompany")]
    public string ReturnExpressCompany { get; set; } = string.Empty;
    
    [JsonProperty("returnPickupCode")]
    public string ReturnPickupCode { get; set; } = string.Empty;    

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