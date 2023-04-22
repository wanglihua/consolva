using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderLogisticsTrack;

public class OrderLogisticsTrackParam {
    [JsonProperty("orderFirstModeId")]
    public int OrderFirstModeId { get; set; }
}

public class OrderLogisticsTrackParamValidator : AbstractValidator<OrderLogisticsTrackParam> {
    public OrderLogisticsTrackParamValidator() {
        RuleFor(e => e.OrderFirstModeId).Must(p => p > 0).WithMessage("查看跟踪明细，订单头程物流ID不能为空！！");
    }
}