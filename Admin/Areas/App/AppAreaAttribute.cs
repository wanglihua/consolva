using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App;

public class AppArea {
    public static string Name => "App";
}

/// <summary>
/// 后台APP接口
/// </summary>
public class AppAreaAttribute : AreaAttribute {
    public AppAreaAttribute() : base(AppArea.Name) {
    }
}