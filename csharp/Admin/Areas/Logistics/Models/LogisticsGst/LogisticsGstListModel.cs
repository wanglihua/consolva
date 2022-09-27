using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGst;

public class LogisticsGstListModel : ListModel
{
    public LogisticsGstListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsGstInfo> GstList { get; set; } = new List<LogisticsGstInfo>();
}