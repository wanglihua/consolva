using shunda.Core.CodeType;

namespace shunda.Service.Pay;

public class PaypalPaymentMethod : StringCodeTypeBase {
    /// <summary>
    /// paypal
    /// </summary>
    public static readonly PaypalPaymentMethod Paypal = new PaypalPaymentMethod("paypal", "paypal", "");

    /// <summary>
    /// credit_card
    /// </summary>
    public static readonly PaypalPaymentMethod CreditCard = new PaypalPaymentMethod("credit_card", "credit_card", "");

    /// <summary>
    /// None
    /// </summary>
    public static readonly PaypalPaymentMethod None = new PaypalPaymentMethod("", "", "");

    private PaypalPaymentMethod(string code, string name, string description) : base(code, name, description) {
    }
}