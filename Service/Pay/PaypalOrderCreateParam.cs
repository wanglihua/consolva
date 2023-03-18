using shunda.Domain.Fee;

namespace shunda.Service.Pay;

public class PaypalOrderCreateParam {
    public int OrderId { get; set; }

    public int CouponId { get; set; }

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
}