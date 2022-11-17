using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

public class LogisticsGoodsTradeIndexModel
{
    [BindNever]
    public int StatusId { get; set; } // todo a 看看是不是可以改成 Status

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetInnerList();
}