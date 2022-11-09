using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

public class ManualFeeApplyListModel : ListModel
{
    public ManualFeeApplyListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ManualFeeApplyInfo> ManualFeeApplyList { get; set; } = new List<ManualFeeApplyInfo>();
}