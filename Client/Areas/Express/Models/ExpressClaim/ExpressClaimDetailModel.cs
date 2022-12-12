using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimDetailModel
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

    [JsonProperty("addDate")]
    public string AddDate { get; set; } = string.Empty;

    [JsonProperty("addTime")]
    public string AddTime { get; set; } = string.Empty;

    // [JsonProperty("claimTime")]
    // public string ClaimTime { get; set; } = string.Empty;

    [JsonProperty("expressCompany")]
    public string ExpressCompany { get; set; } = string.Empty;

    [JsonProperty("warehouseRemark")]
    public string WarehouseRemark { get; set; } = string.Empty;

    [JsonProperty("totalVolume")]
    public string TotalVolume { get; set; } = string.Empty;

    [JsonProperty("totalVolumeWeight")]
    public string TotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("totalActualWeight")]
    public string TotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    /// <summary>
    /// 预报单号
    /// 用于认领时自动填入快递单号
    /// </summary>
    [JsonProperty("expectTrackingNo")]
    public string ExpectTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 预报中的客户备注
    /// 认领时可以参考这个备注
    /// </summary>
    [JsonProperty("expectDescription")]
    public string ExpectDescription { get; set; } = string.Empty;

    /// <summary>
    /// 是不是已经认领了
    /// </summary>
    [JsonProperty("claimed")]
    public bool Claimed { get; set; } = false;

    [JsonProperty("expressGoodsList")]
    public IList<ExpressGoodsItem> ExpressGoodsList { get; set; } = new List<ExpressGoodsItem>();

    [JsonProperty("expressSizeWeightList")]
    public IList<ExpressSizeWeightItem> ExpressSizeWeightList { get; set; } = new List<ExpressSizeWeightItem>();

    public class ExpressGoodsItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("expressId")]
        public int ExpressId { get; set; }

        [JsonProperty("goodsId")]
        public int GoodsId { get; set; } // LogisticsGoodsInfo.Id

        [JsonProperty("goodsName")]
        public string GoodsName { get; set; } = string.Empty;

        [JsonProperty("price")]
        public string Price { get; set; } = string.Empty;

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; } = string.Empty;
    }

    public class ExpressSizeWeightItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("length")]
        public string Length { get; set; } = string.Empty;

        [JsonProperty("width")]
        public string Width { get; set; } = string.Empty;

        [JsonProperty("height")]
        public string Height { get; set; } = string.Empty;

        [JsonProperty("weight")]
        public string Weight { get; set; } = string.Empty;

        [JsonProperty("volume")]
        public string Volume { get; set; } = string.Empty;

        [JsonProperty("volumeWeight")]
        public string VolumeWeight { get; set; } = string.Empty;

        [JsonProperty("packageCount")]
        public int PackageCount { get; set; }
    }
}