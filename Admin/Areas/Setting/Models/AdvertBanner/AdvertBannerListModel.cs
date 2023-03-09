using shunda.Core;
using shunda.Domain.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Setting.Models.AdvertBanner;

public class AdvertBannerListModel : ListModel {
    public AdvertBannerListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string AdvertBannerAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<AdvertBannerInfo> AdvertBannerList { get; set; } = new List<AdvertBannerInfo>();
}