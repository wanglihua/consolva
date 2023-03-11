using FluentValidation;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;

public class LogisticsModeSettingSaveModelValidator : AbstractValidator<LogisticsModeSettingSaveModel> {
    public LogisticsModeSettingSaveModelValidator() { 
        RuleFor(e => e.VolumeWeightDenominator).Must(p => p > 0).WithMessage("默认体积重分母不能为空！");

        RuleFor(e => e.HasFreePackageLimit).NotNull().NotEmpty().WithMessage("是否有免费包裹数量限制不能为空！");



        RuleFor(e => e.HasExtraPackageFeeCapAmount).NotNull().NotEmpty().WithMessage("是否有额外包裹收费封顶金额不能为空！");


    }
}