using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPick;

public class CouponPickListModel
{
    [JsonProperty("batchList")]
    public IList<CouponPickListItem> BatchList { get; set; } = new List<CouponPickListItem>();
}