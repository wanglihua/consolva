using shunda.Core;
using shunda.Service.Fee;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;

public class LogisticsOrderPayDateStatListModel : ListModel
{
    public LogisticsOrderPayDateStatListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsOrderPayDateStatItem> DateStatList { get; set; } = new List<LogisticsOrderPayDateStatItem>();
}