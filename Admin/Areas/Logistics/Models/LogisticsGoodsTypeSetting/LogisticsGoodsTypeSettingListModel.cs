using shunda.Core;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTypeSetting;

public class LogisticsGoodsTypeSettingListModel : ListModel
{
    public LogisticsGoodsTypeSettingListModel(ListPager listPager) : base(listPager)
    {
    }

    public string LogisticsGoodsTypeSettingAlias = string.Empty;    

    [BindNever]
    public IList<LogisticsGoodsTypeSettingInfo> LogisticsGoodsTypeSettingList { get; set; } = new List<LogisticsGoodsTypeSettingInfo>();
}