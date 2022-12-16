using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Coupon;

// 这个里面包含 地址，余额，积分，支付明细等等的

public class CouponAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Coupon";

    public CouponAreaAttribute() : base(Name)
    {
    }
}