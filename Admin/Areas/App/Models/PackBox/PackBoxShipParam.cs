using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App.Models.PackBox;

public class PackBoxShipParam {
    [BindProperty(Name = "packBoxCode")]
    public string PackBoxCode { get; set; } = string.Empty;
}

public class PackBoxShipParamValidator : AbstractValidator<PackBoxShipParam> {
    public PackBoxShipParamValidator() {
        RuleFor(e => e.PackBoxCode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("箱号不能为空！");
    }
}