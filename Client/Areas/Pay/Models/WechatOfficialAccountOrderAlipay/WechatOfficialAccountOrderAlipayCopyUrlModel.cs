using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;

public class WechatOfficialAccountAlipayCopyUrlModel {
    [BindNever]
    public string Url { get; set; } = string.Empty;
}