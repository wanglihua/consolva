namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderPaypal;

public class WechatOfficialAccountOrderPaypalProcessModel {
    public string PaymentId { get; set; } = string.Empty;

    public bool HasPaypalError { get; set; } = false;
}