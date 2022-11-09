using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApprove;

public class FeeWriteOffApproveListModel : ListModel
{
    public FeeWriteOffApproveListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeWriteOffApplyInfo> FeeWriteOffApplyList { get; set; } = new List<FeeWriteOffApplyInfo>();
}