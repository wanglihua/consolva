using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderClientListParam {
    [JsonProperty("query")]
    public string? Query { get; set; }

    [JsonProperty("lastId")]
    public int LastId { get; set; }

    [BindNever]
    [JsonIgnore]
    public int CustomerId { get; set; }

    [BindNever]
    [JsonIgnore]
    public LogisticsOrderStatusClientCate Cate { get; set; } = LogisticsOrderStatusClientCate.None;
}

public class LogisticsOrderClientListParamWebValidator : AbstractValidator<LogisticsOrderClientListParam> {
    public LogisticsOrderClientListParamWebValidator() {
        RuleFor(e => e.Query).MaximumLength(100).WithMessage("查询字符串太长！");
        RuleFor(e => e.LastId).Must(p => p >= 0).WithMessage("前面列表末尾数据Id不能为空！");
    }
}

public class LogisticsOrderClientListParamServiceValidator : LogisticsOrderClientListParamWebValidator {
    public LogisticsOrderClientListParamServiceValidator() {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户ID不能为空！");
        RuleFor(e => e.Cate).Must(p => p != null && p != LogisticsOrderStatusClientCate.None).WithMessage("订单列表客户端分类不能为空！");
    }
}