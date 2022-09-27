using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGst;

public class LogisticsGstDetailModel
{
    public LogisticsGstInfo LogisticsGst { get; set; } = new LogisticsGstInfo();

    [BindNever]
    public IList<FeeCalcBaseType> FeeCalcBaseTypeList { get; set; } = new List<FeeCalcBaseType>();

    [BindNever]
    public IList<LogisticsGstStatus> GstStatusList { get; set; } = new List<LogisticsGstStatus>();
}