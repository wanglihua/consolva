using FluentValidation;

namespace shunda.Service.Pay;

public class PaypalOrderProcessParam {
    public string PayerId { get; set; } = string.Empty;

    public string OutTradeNo { get; set; } = string.Empty;
}

public class PaypalOrderProcessParamValidator : AbstractValidator<PaypalOrderProcessParam> {
    public PaypalOrderProcessParamValidator() {
        RuleFor(e => e.PayerId).NotNull().NotEmpty().WithMessage("PayerId 不能为空！");
        RuleFor(e => e.OutTradeNo).NotNull().NotEmpty().WithMessage("OutTradeNo 不能为空！");
    }
}