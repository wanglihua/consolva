using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBanner;

public class AdvertBannerSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int AdvertBannerId { get; set; }

    /// <summary>
    /// 位置，非空
    /// </summary>
    [ModelBinder(typeof(AdvertBannerLocModelBinder))]
    public AdvertBannerLoc Loc { get; set; } = AdvertBannerLoc.None;

    /// <summary>
    /// 名称，非空
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否客户端显示，非空
    /// </summary>
    public bool ClientShow { get; set; } = false;
}