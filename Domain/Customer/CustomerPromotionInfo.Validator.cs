using FluentValidation;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户推广 实体验证器
/// </summary>
public class CustomerPromotionInfoValidator : AbstractValidator<CustomerPromotionInfo> {
    public CustomerPromotionInfoValidator() {
        RuleFor(e => e.Parent).Must(p => p != null).WithMessage("父id不能为空！");
        RuleFor(e => e.OpenId).MaximumLength(100).WithMessage("open id长度不能大于100！");
    }
}