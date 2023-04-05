using FluentValidation;
using shunda.Domain.Fee;

namespace shunda.Service.Pay;

public class PaypalChargeCreateParam {
    public int CustomerId { get; set; }

    public decimal Amount { get; set; }

    public CurrencyType Currency { get; set; } = CurrencyType.None;

    public PaypalPaymentMethod PaymentMethod { get; set; } = PaypalPaymentMethod.None;

    /// <summary>
    /// 这个URL已经带有 域名 等信息
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// 这个URL已经带有 域名 等信息
    /// </summary>
    public string CancelUrl { get; set; } = string.Empty;

    public string OutTradeNo { get; set; } = string.Empty;
}

public class PaypalChargeCreateParamValidator : AbstractValidator<PaypalChargeCreateParam> {
    public PaypalChargeCreateParamValidator() {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户ID不能为空！");
        RuleFor(e => e.Amount).Must(p => p > 0).WithMessage("充值金额不能为空或小于0！");
        RuleFor(e => e.Currency).Must(p => p != null && p != CurrencyType.None).WithMessage("货币类型不能为空！");
        RuleFor(e => e.PaymentMethod).Must(p => p != null && p != PaypalPaymentMethod.None).WithMessage("支付方式不能为空！");
        RuleFor(e => e.ReturnUrl).NotNull().NotEmpty().WithMessage("return url 不能为空！");
        RuleFor(e => e.CancelUrl).NotNull().NotEmpty().WithMessage("cancel url 不能为空！");
        RuleFor(e => e.OutTradeNo).NotNull().NotEmpty().WithMessage("out trade no 不能为空！");
    }
}