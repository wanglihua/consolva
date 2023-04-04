using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App.Models.PackBox;

public class PackBoxShipParam {
    [BindProperty(Name = "packBoxId")]
    public int PackBoxId { get; set; }
}

public class PackBoxShipParamValidator : AbstractValidator<PackBoxShipParam> {
    public PackBoxShipParamValidator() {
        RuleFor(e => e.PackBoxId).Must(p => p > 0).WithMessage("箱号ID不能为空！");
    }
}