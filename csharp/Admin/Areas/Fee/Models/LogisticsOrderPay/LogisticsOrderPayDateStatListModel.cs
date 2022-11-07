using shunda.Core;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;

public class LogisticsOrderPayDateStatListModel : ListModel
{
    public LogisticsOrderPayDateStatListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsOrderPayDateStatListItem> DateStatList { get; set; } = new List<LogisticsOrderPayDateStatListItem>();
}