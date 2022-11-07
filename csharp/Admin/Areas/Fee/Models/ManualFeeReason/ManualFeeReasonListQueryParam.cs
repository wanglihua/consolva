using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeReason;

public class ManualFeeReasonListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(ManualFeeTypeModelBinder))]
    public ManualFeeType? FeeTypeQuery { get; set; }

    [ModelBinder(typeof(ManualFeeReasonStatusModelBinder))]
    public ManualFeeReasonStatus? StatusQuery { get; set; }
}