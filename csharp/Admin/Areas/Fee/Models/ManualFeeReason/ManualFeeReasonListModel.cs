using shunda.Core;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeReason;

public class ManualFeeReasonListModel : ListModel
{
    public ManualFeeReasonListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ManualFeeReasonInfo> ManualFeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();
}