using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.Order;

namespace shunda.Admin.Areas.OrderFee.Models.ManualFeeReason;

public class ManualFeeReasonListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(ManualFeeTypeModelBinder))]
    public ManualFeeType? FeeTypeQuery { get; set; }

    [ModelBinder(typeof(ManualFeeReasonStatusModelBinder))]
    public ManualFeeReasonStatus? StatusQuery { get; set; }
}