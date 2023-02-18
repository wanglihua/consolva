using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinPrice;

public class ExpressCheckinPriceSaveParam {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [JsonProperty("goodsValueInput")]
    public string GoodsValueInput { get; set; } = string.Empty;
}

public class ExpressCheckinPriceSaveParamValidator : AbstractValidator<ExpressCheckinPriceSaveParam> {
    public ExpressCheckinPriceSaveParamValidator() {
        RuleFor(e => e.ExpressId).Must(p => p > 0).WithMessage("快递单Id不能为空！");
        RuleFor(e => e.GoodsValueInput).NotNull().NotEmpty().WithMessage("货品总值不能为空！");
    }
}