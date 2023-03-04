using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayCoupon; 

public class OrderPayCouponHasParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}

public class OrderPayCouponHasParamValidator : AbstractValidator<OrderPayCouponHasParam> {
    public OrderPayCouponHasParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单Id不能为空！");
    }
}