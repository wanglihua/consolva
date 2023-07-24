using FluentValidation;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionSaveModelValidator : AbstractValidator<CustomerPromotionSaveModel> {
    public CustomerPromotionSaveModelValidator() { 
        // RuleFor(e => e.Parent).Must(p => p > 0).WithMessage("父id不能为空！");


        RuleFor(e => e.OpenId).MaximumLength(100).WithMessage("open id长度不能大于100！");


    }
}