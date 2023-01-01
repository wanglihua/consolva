using Newtonsoft.Json;
using shunda.Client.Areas.Express.Models.ExpressCheckin;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinDetail;

public class ExpressCheckinDetailModel {
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

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    [JsonProperty("claimDate")]
    public string ClaimDate { get; set; } = string.Empty;

    // 客户端暂时没用起来
    // [JsonProperty("customerRemark")]
    [JsonIgnore]
    public string CustomerRemark { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    // 客户端暂时没用起来
    // [JsonProperty("customerRemark")]
    [JsonIgnore]
    public string ServiceRemark { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("waitOrder")]
    public bool WaitOrder { get; set; } = false;

    [JsonProperty("goodsCount")]
    public int GoodsCount { get; set; }

    [JsonProperty("goodsList")]
    public IList<ExpressCheckinGoodsListItem> GoodsList = new List<ExpressCheckinGoodsListItem>();

    #region size weight

    [JsonProperty("sizeWeightList")]
    public IList<ExpressCheckinDetailSizeWeightListItem> SizeWeightList = new List<ExpressCheckinDetailSizeWeightListItem>();

    [JsonProperty("totalVolume")]
    public string TotalVolume { get; set; } = string.Empty;

    [JsonProperty("totalVolumeWeight")]
    public string TotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("totalActualWeight")]
    public string TotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    #endregion

    #region photo

    [JsonProperty("totalPhotoCount")]
    public int TotalPhotoCount { get; set; }

    [JsonProperty("photoTypeList")]
    public IList<ExpressCheckinDetailPhotoTypeListItem> PhotoTypeList = new List<ExpressCheckinDetailPhotoTypeListItem>();

    #endregion
}