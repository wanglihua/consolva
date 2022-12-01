using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefund;

public class FeeRefundListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(FeeRefundTypeModelBinder))]
    public FeeRefundType? TypeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? ReasonQuery { get; set; }

    [ModelBinder(typeof(FeeRefundStatusModelBinder))]
    public FeeRefundStatus? StatusQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}