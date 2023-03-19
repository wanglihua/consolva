using FluentValidation;

namespace shunda.Service.Pay;

public class PaypalChargeProcessParam {
    public string PayerId { get; set; } = string.Empty;

    public string OutTradeNo { get; set; } = string.Empty;
}

public class PaypalChargeProcessParamValidator : AbstractValidator<PaypalChargeProcessParam> {
    public PaypalChargeProcessParamValidator() {
        RuleFor(e => e.PayerId).NotNull().NotEmpty().WithMessage("PayerId 不能为空！");
        RuleFor(e => e.OutTradeNo).NotNull().NotEmpty().WithMessage("OutTradeNo 不能为空！");
    }
}