using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay;

public class WechatOfficialAccountChargeAlipayReturnModel {
    [BindNever]
    public bool Success { get; set; }

    [BindNever]
    public string Message { get; set; } = string.Empty;
}