using shunda.Core;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeFeeRate;

public class LogisticsModeFeeRateListModel : ListModel
{
    public LogisticsModeFeeRateListModel(ListPager listPager) : base(listPager)
    {
    }

    public string LogisticsModeFeeRateAlias = string.Empty;
    public string ModeAlias { get; set; } = string.Empty;
    public string FeeRateAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsModeFeeRateInfo> LogisticsModeFeeRateList { get; set; } = new List<LogisticsModeFeeRateInfo>();
}