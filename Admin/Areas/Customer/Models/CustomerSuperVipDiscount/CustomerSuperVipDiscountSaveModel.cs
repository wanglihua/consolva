using FluentValidation;

namespace shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;

public class CustomerSuperVipDiscountSaveModel {
    public int DiscountId { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public decimal Discount { get; set; }

    public string? Description { get; set; }
}

public class CustomerSuperVipDiscountSaveModelValidator : AbstractValidator<CustomerSuperVipDiscountSaveModel> {
    public CustomerSuperVipDiscountSaveModelValidator() {
        RuleFor(e => e.CustomerCode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("客户ID不能为空！");
        RuleFor(e => e.Discount).Must(p => p > 0).WithMessage("折扣不能为空且应大于0！");
        RuleFor(e => e.Discount).Must(p => p <= 10m).WithMessage("折扣不能大于10%！");
        RuleFor(e => e.Description).MaximumLength(500).WithMessage("描述不能最长不能超过500！");
    }
}