using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.RefundReason;

public class RefundReasonListModel : ListModel
{
    public RefundReasonListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeRefundReasonInfo> RefundReasonList { get; set; }
}