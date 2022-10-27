using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.LogisticsOrder;

public class LogisticsOrderAreaAttribute : AreaAttribute
{
    public static readonly string Name = "LogisticsOrder";

    public LogisticsOrderAreaAttribute() : base(Name)
    {
    }
}