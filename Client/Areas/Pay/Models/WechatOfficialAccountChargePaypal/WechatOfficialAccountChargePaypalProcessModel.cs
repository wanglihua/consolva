namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargePaypal;

public class WechatOfficialAccountChargePaypalProcessModel {
    public string PaymentId { get; set; } = string.Empty;

    public bool HasPaypalError { get; set; } = false;
}