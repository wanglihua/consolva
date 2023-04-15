using FluentValidation;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK快递单事件 实体验证器
/// </summary>
public class SeventeenTrackNumberEventInfoValidator : AbstractValidator<SeventeenTrackNumberEventInfo> {
    public SeventeenTrackNumberEventInfoValidator() { 
        RuleFor(e => e.TrackNumber).Must(p => p != null).WithMessage("17TRACK注册快递单不能为空！");

        RuleFor(e => e.EventTime).NotNull().WithMessage("时间不能为空！");

        RuleFor(e => e.EventDesc).NotNull().WithMessage("描述不能为空！");
        RuleFor(e => e.EventDesc).MaximumLength(100).WithMessage("描述长度不能大于100！");

        RuleFor(e => e.Stage).NotNull().WithMessage("里程碑状态不能为空！");
        RuleFor(e => e.Stage).MaximumLength(30).WithMessage("里程碑状态长度不能大于30！");

    }
}