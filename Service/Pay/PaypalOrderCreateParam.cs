using PayPal.Api;
using shunda.Domain.Fee;

namespace shunda.Service.Pay; 

public class PaypalOrderCreateParam {
    public int OrderId { get; set; }
    
    public int CouponId { get; set; }
    
    public CurrencyType
}