using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatAppOrderPayCustomerAccount;

public class WechatAppOrderPayCustomerAccountDetailParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("couponId")]
    public int CouponId { get; set; }

    [JsonProperty("scoreDeduct")]
    public bool ScoreDeduct { get; set; }
}

public class WechatAppOrderPayCustomerAccountDetailParamValidator : AbstractValidator<WechatAppOrderPayCustomerAccountDetailParam> {
    public WechatAppOrderPayCustomerAccountDetailParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
    }
}