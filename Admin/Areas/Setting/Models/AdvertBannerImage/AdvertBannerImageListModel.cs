using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageListModel {
    [BindNever]
    public string AdvertBannerImageAlias { get; set; } = string.Empty;

    [BindNever]
    public string BannerAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<AdvertBannerImageInfo> AdvertBannerImageList { get; set; } = new List<AdvertBannerImageInfo>();
}