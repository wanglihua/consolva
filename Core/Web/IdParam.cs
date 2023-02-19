using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Core.Web;

public class IdParam {
    [BindProperty(Name = "id")]
    [JsonProperty("id")]
    [Required(ErrorMessage = "id不能为空！")]
    public int Id { get; set; } = 0;
}

public class IdParamValidator : AbstractValidator<IdParam> {
    public IdParamValidator() {
        RuleFor(e => e.Id).Must(p => p > 0).WithMessage("id不能为空！");
    }
}