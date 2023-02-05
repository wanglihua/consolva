using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Setting;

public class SettingArea {
    public static string Name => "Setting";
}

public class SettingAreaAttribute : AreaAttribute {
    public SettingAreaAttribute() : base(SettingArea.Name) {
    }
}