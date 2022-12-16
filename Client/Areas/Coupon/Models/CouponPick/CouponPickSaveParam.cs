using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPick;

public class CouponPickSaveParam
{
    [Required(ErrorMessage = "优惠券批次不能为空！")]
    [JsonProperty("batchId")]
    public int BatchId { get; set; }
}