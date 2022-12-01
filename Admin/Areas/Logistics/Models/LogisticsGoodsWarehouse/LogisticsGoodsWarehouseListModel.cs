using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

public class LogisticsGoodsWarehouseListModel : ListModel
{
    public LogisticsGoodsWarehouseListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsGoodsInfo> LogisticsGoodsList { get; set; } = new List<LogisticsGoodsInfo>();
}