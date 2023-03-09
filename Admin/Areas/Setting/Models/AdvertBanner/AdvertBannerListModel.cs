using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBanner;

public class AdvertBannerListModel {
    [BindNever]
    public string AdvertBannerAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<AdvertBannerInfo> AdvertBannerList { get; set; } = new List<AdvertBannerInfo>();
}