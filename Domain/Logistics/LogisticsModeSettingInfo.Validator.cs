using FluentValidation;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流配置 实体验证器
/// </summary>
public class LogisticsModeSettingInfoValidator : AbstractValidator<LogisticsModeSettingInfo> {
    public LogisticsModeSettingInfoValidator() {
        RuleFor(e => e.VolumeWeightDenominator).Must(p => p > 0).WithMessage("默认体积重分母不能为空！");
    }
}