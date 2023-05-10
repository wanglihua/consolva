using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageDetail;

public class CustomerMessageDetailParam {
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("messageId")]
    public int MessageId { get; set; }
}

public class CustomerMessageDetailParamValidator : AbstractValidator<CustomerMessageDetailParam> {
    public CustomerMessageDetailParamValidator() {
        // RuleFor(e => e.Code).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("登录code不能为空！"); 这个条件后面加上
        RuleFor(e => e.MessageId).Must(p => p > 0).WithMessage("消息ID不能为空！");
    }
}