using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimPickParam {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [JsonProperty("trackingNoInput")]
    public string TrackingNoInput { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;
}

public class ExpressClaimPickParamValidator : AbstractValidator<ExpressClaimPickParam> {
    public ExpressClaimPickParamValidator() {
        RuleFor(e => e.ExpressId).Must(p => p > 0).WithMessage("快递单ID不能为空！");
        RuleFor(e => e.TrackingNoInput).NotNull().NotEmpty().WithMessage("快递单号不能为空！");
        RuleFor(e => e.GoodsValue).NotNull().NotEmpty().WithMessage("货品总值不能为空！");
    }
}