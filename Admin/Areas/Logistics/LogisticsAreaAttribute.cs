using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Logistics;

public class LogisticsAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Logistics";

    public LogisticsAreaAttribute() : base(Name)
    {
    }
}