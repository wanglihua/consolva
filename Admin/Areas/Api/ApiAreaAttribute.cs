using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Api;

public class ApiAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Api";

    public ApiAreaAttribute() : base(Name)
    {
    }
}