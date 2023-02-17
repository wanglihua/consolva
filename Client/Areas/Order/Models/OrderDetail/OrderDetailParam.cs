using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}

public class OrderDetailParamValidator : AbstractValidator<OrderDetailParam> {
    public OrderDetailParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单id不能为空！");
    }
}