using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App;

/// <summary>
/// 后台APP接口
/// </summary>
public class AppAreaAttribute : AreaAttribute
{
    public static readonly string Name = "App";

    public AppAreaAttribute() : base(Name)
    {
    }
}