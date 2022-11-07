using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee;

public class OrderFeeAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Fee";

    public OrderFeeAreaAttribute() : base(Name)
    {
    }
}