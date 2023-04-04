using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App.Models.PackBox;

public class PackBoxQueryParam {
    [BindProperty(Name = "packBoxCode")]
    public string PackBoxCode { get; set; } = string.Empty;
}

public class PackBoxQueryParamValidator : AbstractValidator<PackBoxQueryParam> {
    public PackBoxQueryParamValidator() {
        RuleFor(e => e.PackBoxCode).NotNull().NotEmpty().WithMessage("箱号不能为空！");
    }
}