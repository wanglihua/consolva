using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBanner;

public class AdvertBannerIndexModel {
    [BindNever]
    public IList<AdvertBannerLoc> LocList { get; set; } = new List<AdvertBannerLoc>();
}