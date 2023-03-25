using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Models.WechatAppLogin;

public class WechatAppLoginParam {
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("agreed")]
    public bool Agreed { get; set; } = false;
}

public class WechatAppLoginParamValidator : AbstractValidator<WechatAppLoginParam> {
    public WechatAppLoginParamValidator() {
        RuleFor(e => e.Code).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("code不能为空！");
    }
}