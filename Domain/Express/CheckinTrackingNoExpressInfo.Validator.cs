using FluentValidation;

namespace shunda.Domain.Express;

/// <summary>
/// 入库跟踪单号快递 实体验证器
/// </summary>
public class CheckinTrackingNoExpressInfoValidator : AbstractValidator<CheckinTrackingNoExpressInfo> {
    public CheckinTrackingNoExpressInfoValidator() {
        RuleFor(e => e.CheckinTrackingNo).NotNull().WithMessage("入库跟踪号不能为空！");
        RuleFor(e => e.CheckinTrackingNo).MaximumLength(20).WithMessage("入库跟踪号长度不能大于20！");

        RuleFor(e => e.TrackingNo).NotNull().WithMessage("快递单号不能为空！");
        RuleFor(e => e.TrackingNo).MaximumLength(40).WithMessage("快递单号长度不能大于40！");

        RuleFor(e => e.Express).Must(p => p != null).WithMessage("快递单id不能为空！");
    }
}