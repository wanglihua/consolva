using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Models.Home; 

public class HomeClientHomeDataParam {
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
}

public class HomeClientHomeDataParamValidator : AbstractValidator<HomeClientHomeDataParam> {
    public HomeClientHomeDataParamValidator() {
        RuleFor(e => e.Code).NotNull().NotEmpty().WithMessage("code不能为空！");
    }
}