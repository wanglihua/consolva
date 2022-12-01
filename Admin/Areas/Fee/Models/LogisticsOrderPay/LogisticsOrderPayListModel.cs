using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;

public class LogisticsOrderPayListModel : ListModel
{
    public LogisticsOrderPayListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; } = new List<LogisticsOrderPayInfo>();
}