using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageDetail;

public class CustomerMessageDetailParam {
    [JsonProperty("messageId")]
    public int MessageId { get; set; }
}

public class CustomerMessageDetailParamValidator : AbstractValidator<CustomerMessageDetailParam> {
    public CustomerMessageDetailParamValidator() {
        RuleFor(e => e.MessageId).Must(p => p > 0).WithMessage("消息ID不能为空！");
    }
}