using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.AutoApi;

public class AutoApiArea {
    public static string Name => "AutoApi";
}

/// <summary>
/// 供自动测量机器调用，这里暂时还用不上
/// </summary>
public class AutoApiAreaAttribute : AreaAttribute {
    public AutoApiAreaAttribute() : base(AutoApiArea.Name) {
    }
}