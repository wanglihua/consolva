using FluentValidation;

namespace shunda.Admin.Areas.Customer.Models.CustomerAddress;

public class CustomerAddressReceiveDetailParam {
    public int Id { get; set; }

    public int CustomerId { get; set; }
}

public class CustomerAddressReceiveDetailParamValidator : AbstractValidator<CustomerAddressReceiveDetailParam> {
    public CustomerAddressReceiveDetailParamValidator() {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户ID不能为空！");
    }
}