using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsInsurance;

public class LogisticsInsuranceDetailModel
{
    public LogisticsInsuranceInfo LogisticsInsurance { get; set; } = new LogisticsInsuranceInfo();

    [BindNever]
    public IList<FeeCalcBaseType> FeeCalcBaseTypeList { get; set; } = new List<FeeCalcBaseType>();

    [BindNever]
    public IList<LogisticsInsuranceType> LogisticsInsuranceTypeList { get; set; } = new List<LogisticsInsuranceType>();

    [BindNever]
    public IList<LogisticsInsuranceStatus> LogisticsInsuranceStatusList { get; set; } = new List<LogisticsInsuranceStatus>();
}