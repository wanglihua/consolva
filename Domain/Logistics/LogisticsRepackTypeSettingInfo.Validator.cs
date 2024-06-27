using FluentValidation;

namespace shunda.Domain.Logistics;

/// <summary>
/// 打包类型设置 实体验证器
/// </summary>
public class LogisticsRepackTypeSettingInfoValidator : AbstractValidator<LogisticsRepackTypeSettingInfo>
{
    public LogisticsRepackTypeSettingInfoValidator()
    {
        RuleFor(e => e.Type).NotNull().WithMessage("打包类型不能为空！");
        RuleFor(e => e.Type).Must(p => p != LogisticsRepackType.None).WithMessage("打包类型不能为空！");

        RuleFor(e => e.Title).NotNull().WithMessage("标题不能为空！");
        RuleFor(e => e.Title).MaximumLength(100).WithMessage("标题长度不能大于100！");

        RuleFor(e => e.Description).NotNull().WithMessage("描述不能为空！");
        RuleFor(e => e.Description).MaximumLength(300).WithMessage("描述长度不能大于300！");

        RuleFor(e => e.OrderNum).Must(p => p > 0).WithMessage("排序不能为空！");

        RuleFor(e => e.Status).NotNull().WithMessage("状态不能为空！");
        RuleFor(e => e.Status).Must(p => p != LogisticsRepackTypeStatus.None).WithMessage("状态不能为空！");
    }
}