using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Wechat;

public class WechatArea {
    public static string Name => "Wechat";
}

/// <summary>
/// 后台APP接口
/// </summary>
public class WechatAreaAttribute : AreaAttribute {
    public WechatAreaAttribute() : base(WechatArea.Name) {
    }
}