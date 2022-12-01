using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundReason;

public class FeeRefundReasonListModel : ListModel
{
    public FeeRefundReasonListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeRefundReasonInfo> RefundReasonList { get; set; } = new List<FeeRefundReasonInfo>();
}