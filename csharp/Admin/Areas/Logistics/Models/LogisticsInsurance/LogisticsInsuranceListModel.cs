using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsInsurance;

public class LogisticsInsuranceListModel : ListModel
{
    public LogisticsInsuranceListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsInsuranceInfo> LogisticsInsuranceList { get; set; } = new List<LogisticsInsuranceInfo>();
}