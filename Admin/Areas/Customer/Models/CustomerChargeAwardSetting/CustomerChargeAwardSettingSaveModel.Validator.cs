using FluentValidation;

namespace shunda.Admin.Areas.Customer.Models.CustomerChargeAwardSetting;

public class CustomerChargeAwardSettingSaveModelValidator : AbstractValidator<CustomerChargeAwardSettingSaveModel> {
    public CustomerChargeAwardSettingSaveModelValidator() {
        RuleFor(e => e.AmountBegin).Must(p => p >= 0).WithMessage("最小充值金额必须大于等于0！");
        RuleFor(e => e.AmountEnd).Must(p => p > 0).WithMessage("最大充值金额必须大于0！");
        RuleFor(e => e.AmountEnd).Must((e, _) => e.AmountEnd > e.AmountBegin).WithMessage("最大充值金额应该大于最小充值金额！");
        RuleFor(e => e.Award).Must(p => p >= 0).WithMessage("奖励金额不能小于0！");
    }
}