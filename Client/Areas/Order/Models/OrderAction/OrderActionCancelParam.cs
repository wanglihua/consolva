using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderAction;

public class OrderActionCancelParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}

public class OrderActionCancelParamValidator : AbstractValidator<OrderActionCancelParam> {
    public OrderActionCancelParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
    }
}