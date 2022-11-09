using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApprove;

public class FeeWriteOffApproveIndexModel
{
    /// <summary>
    /// 核销费用原因列表
    /// </summary>
    [BindNever]
    public IList<ManualFeeReasonInfo> FeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();

    [BindNever]
    public IList<FeeWriteOffApplyStatus> ApplyStatusList { get; set; } = FeeWriteOffApplyStatus.GetList();

    [BindNever]
    public IList<UserInfo> ApplyUserList { get; set; } = new List<UserInfo>();
}