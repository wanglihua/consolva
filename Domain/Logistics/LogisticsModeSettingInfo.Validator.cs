using FluentValidation;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流配置 实体验证器
/// </summary>
public class LogisticsModeSettingInfoValidator : AbstractValidator<LogisticsModeSettingInfo> {
    public LogisticsModeSettingInfoValidator() { 
        RuleFor(e => e.VolumeWeightDenominator).Must(p => p > 0).WithMessage("默认体积重分母不能为空！");

        RuleFor(e => e.HasFreePackageLimit).NotNull().NotEmpty().WithMessage("是否有免费包裹数量限制不能为空！");



        RuleFor(e => e.HasExtraPackageFeeCapAmount).NotNull().NotEmpty().WithMessage("是否有额外包裹收费封顶金额不能为空！");


    }
}