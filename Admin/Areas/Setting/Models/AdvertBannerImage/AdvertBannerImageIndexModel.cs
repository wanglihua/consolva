using shunda.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageIndexModel {
    [BindNever]
    public AdvertBannerInfo AdvertBanner { get; set; } = new();
}