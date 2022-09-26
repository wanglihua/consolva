using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRate;

public class LogisticsFeeRateDetailModel
{
    public LogisticsFeeRateInfo LogisticsFeeRate { get; set; } = new LogisticsFeeRateInfo();

    [BindNever]
    public IList<VolumeWeightOrientationType> VolumeWeightOrientationList { get; set; } = new List<VolumeWeightOrientationType>();

    [BindNever]
    public IList<LogisticsFeeRateStatus> StatusList { get; set; } = new List<LogisticsFeeRateStatus>();
}