using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Express;

public class ExpressArea : AreaAttribute
{
    public static readonly string Name = "Express";

    public ExpressArea() : base(Name)
    {
    }
}