using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRate;

public class LogisticsFeeRateDetailModel
{
    public LogisticsFeeRateInfo LogisticsFeeRate { get; set; } = new LogisticsFeeRateInfo();

    [BindNever]
    public IList<VolumeWeightResultType> VolumeWeightResultTypeList { get; set; } = new List<VolumeWeightResultType>();

    [BindNever]
    public IList<LogisticsFeeRateStatus> StatusList { get; set; } = new List<LogisticsFeeRateStatus>();
}