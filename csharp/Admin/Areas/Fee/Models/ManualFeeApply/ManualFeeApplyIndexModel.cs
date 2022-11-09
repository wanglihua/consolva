using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

public class ManualFeeApplyIndexModel
{
    /// <summary>
    /// 手工费用原因列表
    /// </summary>
    [BindNever]
    public IList<ManualFeeReasonInfo> FeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();

    [BindNever]
    public IList<ManualFeeApplyStatus> ApplyStatusList { get; set; } = ManualFeeApplyStatus.GetList();

    [BindNever]
    public IList<FeeApplyDataViewScope> ScopeList { get; set; } = FeeApplyDataViewScope.GetList();
}