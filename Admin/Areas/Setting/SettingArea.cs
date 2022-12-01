using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Setting;

public class SettingAreaAttribute : AreaAttribute
{
    public static readonly string Name = "Setting";

    public SettingAreaAttribute() : base(Name)
    {
    }
}