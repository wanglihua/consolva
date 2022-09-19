using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsFeeRate;

namespace shunda.Admin.Models.LogisticsFeeRate;

public class LogisticsFeeRateDetailModel
{
    public LogisticsFeeRateInfo LogisticsFeeRate { get; set; } = new LogisticsFeeRateInfo();

    [BindNever]
    public IList<VolumeWeightOrientationType> VolumeWeightOrientationList { get; set; } = new List<VolumeWeightOrientationType>();

    [BindNever]
    public IList<LogisticsFeeRateStatus> StatusList { get; set; } = new List<LogisticsFeeRateStatus>();
}