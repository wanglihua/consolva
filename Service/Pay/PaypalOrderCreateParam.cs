using PayPal.Api;
using shunda.Domain.Fee;

namespace shunda.Service.Pay; 

public class PaypalOrderCreateParam {
    public int OrderId { get; set; }
    
    public int CouponId { get; set; }
    
    public CurrencyType Currency { get; set; } = CurrencyType.None;

    public string ReturnUrl { get; set; } = string.Empty;

    public string ProcessUrl { get; set; } = string.Empty;
}