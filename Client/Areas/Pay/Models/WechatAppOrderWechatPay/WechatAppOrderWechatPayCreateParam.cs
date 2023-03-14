using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatAppOrderWechatPay;

public class WechatAppOrderWechatPayCreateParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("couponId")]
    public int CouponId { get; set; }
}

public class WechatAppOrderWechatPayCreateParamValidator : AbstractValidator<WechatAppOrderWechatPayCreateParam> {
    public WechatAppOrderWechatPayCreateParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
    }
}