using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;

public class WechatOfficialAccountOrderAlipayReturnModel {
    [BindNever]
    public bool Success { get; set; }

    [BindNever]
    public string Message { get; set; } = string.Empty;

    [BindNever]
    public LogisticsOrderInfo Order { get; set; } = new();
}