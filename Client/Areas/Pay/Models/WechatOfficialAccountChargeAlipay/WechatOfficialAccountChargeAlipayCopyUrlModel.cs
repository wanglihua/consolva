using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay;

public class WechatOfficialAccountChargeAlipayCopyUrlModel {
    [BindNever]
    public string Url { get; set; } = string.Empty;
}