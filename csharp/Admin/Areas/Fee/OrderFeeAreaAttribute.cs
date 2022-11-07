using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.OrderFee;

public class OrderFeeAreaAttribute : AreaAttribute
{
    public static readonly string Name = "OrderFee";

    public OrderFeeAreaAttribute() : base(Name)
    {
    }
}