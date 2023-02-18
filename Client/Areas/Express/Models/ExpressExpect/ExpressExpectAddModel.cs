using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectAddModel {
    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsValue")]
    public string GoodsValue { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }
}

public class ExpressExpectAddModelValidator : AbstractValidator<ExpressExpectAddModel> {
    public ExpressExpectAddModelValidator() {
        RuleFor(e => e.TrackingNo).NotNull().NotEmpty().WithMessage("快递单号不能为空！");
        RuleFor(e => e.TrackingNo).MaximumLength(30).WithMessage("快递单号长度不能大于30！");

        RuleFor(e => e.GoodsValue).NotNull().NotEmpty().WithMessage("货品总值不能为空！");

        RuleFor(e => e.Description).MaximumLength(500).WithMessage("备注长度不能大于500！");
    }
}