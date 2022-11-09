using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApply;

public class FeeWriteOffApplyIndexModel
{
    /// <summary>
    /// 核销费用原因列表
    /// </summary>
    [BindNever]
    public IList<ManualFeeReasonInfo> FeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();

    [BindNever]
    public IList<FeeWriteOffApplyStatus> ApplyStatusList { get; set; } = new List<FeeWriteOffApplyStatus>();

    [BindNever]
    public IList<FeeApplyDataViewScope> ScopeList { get; set; } = FeeApplyDataViewScope.GetList();
}