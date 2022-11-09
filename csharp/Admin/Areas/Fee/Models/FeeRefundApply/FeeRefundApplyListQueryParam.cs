using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApply;

public class FeeRefundApplyListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(FeeRefundTypeModelBinder))]
    public FeeRefundType TypeQuery { get; set; }

    public string OrderCodeQuery { get; set; }

    public string CustomerCodeQuery { get; set; }

    public string ReasonQuery { get; set; }

    [ModelBinder(typeof(FeeRefundApplyStatusModelBinder))]
    public FeeRefundApplyStatus StatusQuery { get; set; }

    [ModelBinder(typeof(FeeApplyDataViewScopeModelBinder))]
    public FeeApplyDataViewScope ViewScopeQuery { get; set; }
}