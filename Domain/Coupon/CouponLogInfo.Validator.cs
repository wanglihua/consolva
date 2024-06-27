using FluentValidation;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券 实体验证器
/// </summary>
public class CouponLogInfoValidator : AbstractValidator<CouponLogInfo>
{
    public CouponLogInfoValidator()
    {
        RuleFor(e => e.Coupon).Must(p => p != null).WithMessage("优惠券不能为空！");

        RuleFor(e => e.Type).NotNull().WithMessage("类型不能为空！");
        RuleFor(e => e.Type).Must(p => p != CouponLogType.None).WithMessage("类型不能为空！");


        RuleFor(e => e.Description).NotNull().WithMessage("描述不能为空！");
        RuleFor(e => e.Description).MaximumLength(500).WithMessage("描述长度不能大于500！");
    }
}