using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee;

public class OrderFeeAreaAttribute : AreaAttribute
{
    public static readonly string Name = "OrderFee";

    public OrderFeeAreaAttribute() : base(Name)
    {
    }
}