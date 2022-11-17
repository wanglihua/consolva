using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

public class LogisticsGoodsTradeListModel : ListModel

{
    public LogisticsGoodsTradeListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsGoodsInfo> LogisticsGoodsList { get; set; } = new List<LogisticsGoodsInfo>();
}