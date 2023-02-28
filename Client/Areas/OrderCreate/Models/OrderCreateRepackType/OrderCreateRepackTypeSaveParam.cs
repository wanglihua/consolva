using FluentValidation;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateRepackType;

public class OrderCreateRepackTypeSaveParam {
    [JsonProperty("repackTypeId")]
    public int RepackTypeId { get; set; }

    [JsonProperty("remark")]
    public string Remark { get; set; } = string.Empty;
}

public class OrderCreateRepackTypeSaveParamValidator : AbstractValidator<OrderCreateRepackTypeSaveParam> {
    public OrderCreateRepackTypeSaveParamValidator() {
        RuleFor(e => e.RepackTypeId).Must(p => LogisticsRepackType.GetById(p) != LogisticsRepackType.None).WithMessage("打包方式不能为空！");
        RuleFor(e => e.Remark).Must(p => (p?.Length ?? 0) < 300).WithMessage("备注长度不能大于300！");
    }
}