using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

public class LogisticsGoodsTradeIndexModel
{
    [BindNever]
    public LogisticsGoodsStatus Status { get; set; } = LogisticsGoodsStatus.None;

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetInnerList();
}