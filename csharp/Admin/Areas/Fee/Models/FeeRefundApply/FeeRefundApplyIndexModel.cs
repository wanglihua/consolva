using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApply;

public class FeeRefundApplyIndexModel
{
    [BindNever]
    public IList<FeeRefundType> TypeList { get; set; } = FeeRefundType.GetList();

    [BindNever]
    public IList<FeeRefundApplyStatus> StatusList { get; set; } = FeeRefundApplyStatus.GetList();

    [BindNever]
    public IList<FeeApplyDataViewScope> ScopeList { get; set; } = FeeApplyDataViewScope.GetList();
}