using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

public class ManualFeeApproveIndexModel
{
    /// <summary>
    /// 手工费用原因列表
    /// </summary>
    [BindNever]
    public IList<ManualFeeReasonInfo> FeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();

    [BindNever]
    public IList<ManualFeeApplyStatus> ApplyStatusList { get; set; } = ManualFeeApplyStatus.GetList();

    [BindNever]
    public IList<UserInfo> ApplyUserList { get; set; } = new List<UserInfo>();
}