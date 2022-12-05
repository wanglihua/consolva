using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTypeSetting;

public class LogisticsGoodsTypeSettingIndexModel
{
    [BindNever]
    public IList<LogisticsGoodsType> TypeList { get; set; } = new List<LogisticsGoodsType>();

    [BindNever]
    public IList<LogisticsGoodsTypeStatus> StatusList { get; set; } = new List<LogisticsGoodsTypeStatus>();

}