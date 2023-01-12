using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsInsurance;

public class LogisticsInsuranceDetailModel
{
    public LogisticsInsuranceWithContentInfo LogisticsInsurance { get; set; } = new();

    [BindNever]
    public IList<FeeCalcBaseType> FeeCalcBaseTypeList { get; set; } = new List<FeeCalcBaseType>();

    [BindNever]
    public IList<LogisticsInsuranceType> LogisticsInsuranceTypeList { get; set; } = new List<LogisticsInsuranceType>();

    [BindNever]
    public IList<LogisticsInsuranceStatus> LogisticsInsuranceStatusList { get; set; } = new List<LogisticsInsuranceStatus>();
}