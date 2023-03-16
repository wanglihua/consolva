using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;

public class WechatOfficialAccountOrderAlipayCopyUrlModel {
    [BindNever]
    public string Url { get; set; } = string.Empty;
}