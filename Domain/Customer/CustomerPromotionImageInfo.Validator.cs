using FluentValidation;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户推广图片 实体验证器
/// </summary>
public class CustomerPromotionImageInfoValidator : AbstractValidator<CustomerPromotionImageInfo> {
    public CustomerPromotionImageInfoValidator() { 
        RuleFor(e => e.ImageNum).Must(p => p > 0).WithMessage("图片序号不能为空！");

        RuleFor(e => e.ImageFileName).NotNull().WithMessage("图片文件名不能为空！");
        RuleFor(e => e.ImageFileName).MaximumLength(300).WithMessage("图片文件名长度不能大于300！");

    }
}