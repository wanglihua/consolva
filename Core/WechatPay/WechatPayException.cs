namespace shunda.Core.WechatPay;

public class WechatPayException : Exception {
    public WechatPayException(string message) : base(message) {
    }
}