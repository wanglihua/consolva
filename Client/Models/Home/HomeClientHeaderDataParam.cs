using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Models.Home; 

public class HomeClientHeaderDataParam {
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
}

public class HomeClientHeaderDataParamValidator : AbstractValidator<HomeClientHeaderDataParam> {
    public HomeClientHeaderDataParamValidator() {
        RuleFor(e => e.Code).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("code不能为空！");
    }
}