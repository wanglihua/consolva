using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppLogin;

public class WechatAppLoginParam {
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
}

public class WechatAppLoginParamValidator : AbstractValidator<WechatAppLoginParam> {
    public WechatAppLoginParamValidator() {
        RuleFor(e => e.Code).NotNull().NotEmpty().WithMessage("code不能为空！");
    }
}