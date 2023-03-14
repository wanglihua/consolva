using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatAppChargeWechatPay;

public class WechatAppChargeWechatPayCreateParam {
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}

public class WechatAppChargeWechatPayCreateParamValidator : AbstractValidator<WechatAppChargeWechatPayCreateParam> {
    public WechatAppChargeWechatPayCreateParamValidator() {
        RuleFor(e => e.Amount).Must(p => p > 0).WithMessage("充值金额必须大于0！");
    }
}