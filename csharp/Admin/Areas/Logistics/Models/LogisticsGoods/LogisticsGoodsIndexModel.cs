using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsIndexModel
{
    [BindNever]
    public LogisticsGoodsType Type { get; set; } = LogisticsGoodsType.None;
}