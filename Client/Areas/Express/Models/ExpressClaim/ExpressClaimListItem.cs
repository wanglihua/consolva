using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("billCustomerCode")]
    public string BillCustomerCode { get; set; } = string.Empty;

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsType")]
    public string GoodsType { get; set; } = string.Empty;

    [JsonProperty("goodsTypeId")]
    public int GoodsTypeId { get; set; }

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    [JsonProperty("expressCompany")]
    public string ExpressCompany { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;
}