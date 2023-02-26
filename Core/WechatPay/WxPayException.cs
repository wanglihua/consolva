using System;

namespace shunda.Core.WechatPay;

public class WxPayException : Exception {
    public WxPayException(string message) : base(message) {
    }
}