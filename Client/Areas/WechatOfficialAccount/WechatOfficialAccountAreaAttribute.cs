using Microsoft.AspNetCore.Mvc;

namespace shunda.Client.Areas.WechatOfficialAccount;

public class WechatOfficialAccountArea {
    public static string Name => "WechatOfficialAccount";
}

public class WechatOfficialAccountAreaAttribute : AreaAttribute {
    public WechatOfficialAccountAreaAttribute() : base(WechatOfficialAccountArea.Name) {
    }
}