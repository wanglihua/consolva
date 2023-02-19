using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectDeleteParam : ExpressExpectListQueryParam {
    [JsonProperty("id")]
    [Required(ErrorMessage = "id不能为空！")]
    public int Id { get; set; } = 0;
}

public class ExpressExpectDeleteParamValidator : AbstractValidator<ExpressExpectDeleteParam> {
    public ExpressExpectDeleteParamValidator() {
        RuleFor(e => e.Id).Must(p => p > 0).WithMessage("id不能为空！");
    }
}