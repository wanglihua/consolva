using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageDetailModel : AdvertBannerImageSaveModel {
    [BindNever]
    public AdvertBannerInfo Banner { get; set; } = new();

    [BindNever]
    public string FullImageUrl { get; set; } = string.Empty;
}