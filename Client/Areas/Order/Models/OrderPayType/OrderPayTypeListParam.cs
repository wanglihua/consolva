using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayType;

public class OrderPayTypeListParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }

    [JsonProperty("couponId")]
    public int CouponId { get; set; }

    [JsonProperty("scoreDeduct")]
    public bool ScoreDeduct { get; set; }
}

public class OrderPayTypeListParamValidator : AbstractValidator<OrderPayTypeListParam> {
    public OrderPayTypeListParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
    }
}