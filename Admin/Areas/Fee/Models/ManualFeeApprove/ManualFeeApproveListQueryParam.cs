using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

public class ManualFeeApproveListQueryParam : ListQueryParam
{
    public int ApplyUserIdQuery { get; set; }

    public int ManualReasonIdQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(ManualFeeApplyStatusModelBinder))]
    public ManualFeeApplyStatus? StatusQuery { get; set; }
}