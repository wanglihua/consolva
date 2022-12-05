using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.Express;

public class ExpressAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Express";

    public ExpressAreaAttribute() : base(Name)
    {
    }
}