using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponMine;

public class CouponMineListModel {
    [JsonProperty("couponList")]
    public IList<CouponMineListItem> CouponList = new List<CouponMineListItem>();

    [JsonProperty("couponCount")]
    public int CouponCount { get; set; }

    [JsonProperty("hasBatchToPick")]
    public bool HasBatchToPick { get; set; }
}