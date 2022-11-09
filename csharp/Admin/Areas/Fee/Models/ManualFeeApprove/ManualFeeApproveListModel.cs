using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

public class ManualFeeApproveListModel : ListModel
{
    public ManualFeeApproveListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ManualFeeApplyInfo> ManualFeeApplyList { get; set; } = new List<ManualFeeApplyInfo>();
}