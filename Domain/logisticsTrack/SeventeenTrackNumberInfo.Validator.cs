using FluentValidation;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK注册快递单 实体验证器
/// </summary>
public class SeventeenTrackNumberInfoValidator : AbstractValidator<SeventeenTrackNumberInfo> {
    public SeventeenTrackNumberInfoValidator() {
        RuleFor(e => e.TrackingNo).NotNull().WithMessage("快递单号不能为空！");
        RuleFor(e => e.TrackingNo).MaximumLength(30).WithMessage("快递单号长度不能大于30！");

        RuleFor(e => e.CarrierName).NotNull().WithMessage("服务商名称不能为空！");
        RuleFor(e => e.CarrierName).MaximumLength(100).WithMessage("服务商名称长度不能大于100！");

        RuleFor(e => e.Origin).NotNull().WithMessage("运输商识别不能为空！");

        RuleFor(e => e.ErrorMessage).NotNull().WithMessage("错误描述不能为空！");
        RuleFor(e => e.ErrorMessage).MaximumLength(300).WithMessage("错误描述长度不能大于300！");

        RuleFor(e => e.Status).NotNull().WithMessage("状态不能为空！");

        RuleFor(e => e.TrackStatus).NotNull().WithMessage("跟踪状态不能为空！");
        RuleFor(e => e.TrackSubStatus).NotNull().WithMessage("跟踪子状态不能为空！");
    }
}