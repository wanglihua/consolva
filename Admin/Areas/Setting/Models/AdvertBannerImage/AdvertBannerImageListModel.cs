using shunda.Core;
using shunda.Domain.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageListModel : ListModel {
    public AdvertBannerImageListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string AdvertBannerImageAlias { get; set; } = string.Empty;

    [BindNever]
    public string BannerAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<AdvertBannerImageInfo> AdvertBannerImageList { get; set; } = new List<AdvertBannerImageInfo>();
}