using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeFeeRate;

public class LogisticsModeFeeRateIndexModel
{
    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();

}