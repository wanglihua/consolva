using FluentValidation;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;

public class LogisticsModeSettingSaveModelValidator : AbstractValidator<LogisticsModeSettingSaveModel> {
    public LogisticsModeSettingSaveModelValidator() { 
        RuleFor(e => e.VolumeWeightDenominator).Must(p => p > 0).WithMessage("默认体积重分母不能为空！");
    }
}