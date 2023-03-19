using FluentValidation;

namespace shunda.Service.Pay;

public class PaypalOrderCreateParam {
    public int OrderId { get; set; }

    public int CouponId { get; set; }

    public PaypalPaymentMethod PaymentMethod { get; set; } = PaypalPaymentMethod.None;

    /// <summary>
    /// 这个URL已经带有 域名 等信息
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// 这个URL已经带有 域名 等信息
    /// </summary>
    public string CancelUrl { get; set; } = string.Empty;
}

public class PaypalOrderCreateParamValidator : AbstractValidator<PaypalOrderCreateParam> {
    public PaypalOrderCreateParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
        RuleFor(e => e.PaymentMethod).Must(p => p != null && p != PaypalPaymentMethod.None).WithMessage("支付方式不能为空！");
        RuleFor(e => e.ReturnUrl).NotNull().NotEmpty().WithMessage("return url 不能为空！");
        RuleFor(e => e.CancelUrl).NotNull().NotEmpty().WithMessage("cancel url 不能为空！");
    }
}