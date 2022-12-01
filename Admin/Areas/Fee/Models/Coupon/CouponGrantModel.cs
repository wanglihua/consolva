using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Fee.Models.Coupon;

public class CouponGrantModel
{
    [Required(ErrorMessage = "优惠券批次ID不能为空！")]
    public int BatchId { get; set; }

    [Required(ErrorMessage = "客户ID不能为空！")]
    public string CustomerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "发放数量不能为空！")]
    public int Quantity { get; set; } = 1;
}