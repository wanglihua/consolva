using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.OrderFee.Models.ManualChargeReason;

public class ManualChargeReasonListModel : ListModel
{
    public ManualChargeReasonListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ManualChargeReasonInfo> ManualChargeReasonList { get; set; } = new List<ManualChargeReasonInfo>();
}