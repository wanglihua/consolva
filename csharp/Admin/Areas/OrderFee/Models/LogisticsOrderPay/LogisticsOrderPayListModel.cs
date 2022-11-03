using shunda.Core;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.OrderFee.Models.LogisticsOrderPay;

public class LogisticsOrderPayListModel : ListModel
{
    public LogisticsOrderPayListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; } = new List<LogisticsOrderPayInfo>();
}