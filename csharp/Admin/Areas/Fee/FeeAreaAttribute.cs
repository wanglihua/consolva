using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee;

public class FeeAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Fee";

    public FeeAreaAttribute() : base(Name)
    {
    }
}