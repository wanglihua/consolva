using FluentValidation;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;

public class CustomerPromotionImageSaveModelValidator : AbstractValidator<CustomerPromotionImageSaveModel> {
    public CustomerPromotionImageSaveModelValidator() { 
        RuleFor(e => e.ImageNum).Must(p => p > 0).WithMessage("图片序号不能为空！");

        RuleFor(e => e.ImageFileName).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("图片文件名不能为空！");
        RuleFor(e => e.ImageFileName).MaximumLength(300).WithMessage("图片文件名长度不能大于300！");

    }
}