using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;

public class FeeWriteOffApplyListModel : ListModel
{
    public FeeWriteOffApplyListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<FeeWriteOffApplyInfo> FeeWriteOffApplyList { get; set; } = new List<FeeWriteOffApplyInfo>();
}