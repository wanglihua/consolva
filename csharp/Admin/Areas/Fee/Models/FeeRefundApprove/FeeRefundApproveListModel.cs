using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApprove;

public class FeeRefundApproveListModel : ListModel
{
    public FeeRefundApproveListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeRefundApplyInfo> FeeRefundApplyList { get; set; }
}