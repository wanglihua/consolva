using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.HsGoods;

public class HsGoodsListModel : ListModel
{
    public HsGoodsListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<HsGoodsInfo> HsGoodsList { get; set; } = new List<HsGoodsInfo>();
}