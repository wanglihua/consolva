using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.CouponBatch;

public class CouponBatchListQueryParam : ListQueryParam {
    public string? NameQuery { get; set; }

    [ModelBinder(typeof(CouponTypeModelBinder))]
    public CouponType? TypeQuery { get; set; }

    [ModelBinder(typeof(CouponBatchStatusModelBinder))]
    public CouponBatchStatus? StatusQuery { get; set; }

    public DateTime? ExpireBeginQuery { get; set; }

    public DateTime? ExpireEndQuery { get; set; }
}