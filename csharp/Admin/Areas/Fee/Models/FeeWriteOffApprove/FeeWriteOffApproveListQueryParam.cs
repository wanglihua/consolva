using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApprove;

public class LogisticsOrderFeeWriteOffApproveListQueryParam : ListQueryParam
{
    public int ApplyUserIdQuery { get; set; }

    public int ManualReasonIdQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(FeeWriteOffApplyStatusModelBinder))]
    public FeeWriteOffApplyStatus? StatusQuery { get; set; }
}