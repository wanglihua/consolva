using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Wechat;

public class WechatArea {
    public static string Name => "Wechat";
}

/// <summary>
/// 微信相关
/// </summary>
public class WechatAreaAttribute : AreaAttribute {
    public WechatAreaAttribute() : base(WechatArea.Name) {
    }
}