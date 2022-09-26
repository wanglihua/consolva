using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Models.LogisticsFeeRate;

public class LogisticsFeeRateListModel : ListModel
{
    public LogisticsFeeRateListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsFeeRateInfo> LogisticsFeeRateList { get; set; } = new List<LogisticsFeeRateInfo>();
}