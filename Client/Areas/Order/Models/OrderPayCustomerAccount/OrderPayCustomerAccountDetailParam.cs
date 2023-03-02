using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderPayCustomerAccount; 

public class OrderPayCustomerAccountDetailParam {
    [JsonProperty("orderId")]
    public int OrderId { get; set; }
}

public class OrderPayCustomerAccountDetailParamValidator : AbstractValidator<OrderPayCustomerAccountDetailParam> {
    public OrderPayCustomerAccountDetailParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
    }
}