using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApply;

public class FeeRefundApplyListModel : ListModel
{
    public FeeRefundApplyListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeRefundApplyInfo> FeeRefundApplyList { get; set; } = new List<FeeRefundApplyInfo>();
}