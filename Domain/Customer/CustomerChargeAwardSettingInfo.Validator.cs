using FluentValidation;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户充值优惠设置 实体验证器
/// </summary>
public class CustomerChargeAwardSettingInfoValidator : AbstractValidator<CustomerChargeAwardSettingInfo> {
    public CustomerChargeAwardSettingInfoValidator() {
        RuleFor(e => e.AmountBegin).Must(p => p >= 0).WithMessage("最小充值金额必须大于等于0！");
        RuleFor(e => e.AmountEnd).Must(p => p > 0).WithMessage("最大充值金额必须大于0！");
        RuleFor(e => e.AmountEnd).Must((e, _) => e.AmountEnd > e.AmountBegin).WithMessage("最大充值金额应该大于最小充值金额！");
        RuleFor(e => e.Award).Must(p => p >= 0).WithMessage("奖励金额不能小于0！");
    }
}