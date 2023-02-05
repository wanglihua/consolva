using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Coupon;

public class CouponArea {
    public static string Name => "Coupon";
}

public class CouponAreaAttribute : AreaAttribute {
    public CouponAreaAttribute() : base(CouponArea.Name) {
    }
}