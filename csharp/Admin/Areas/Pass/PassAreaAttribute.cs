using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Pass;

public class PassAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Pass";

    public PassAreaAttribute() : base(Name)
    {
    }
}