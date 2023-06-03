using FluentValidation;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerChargeAwardSetting;

public class CustomerChargeAwardSettingSaveModelValidator : AbstractValidator<CustomerChargeAwardSettingSaveModel> {
    public CustomerChargeAwardSettingSaveModelValidator() { 
        RuleFor(e => e.AmountBegin).Must(p => p > 0).WithMessage("最小充值金额不能为空！");

        RuleFor(e => e.AmountEnd).Must(p => p > 0).WithMessage("最大充值金额不能为空！");

        RuleFor(e => e.Award).Must(p => p > 0).WithMessage("奖励金额不能为空！");

    }
}