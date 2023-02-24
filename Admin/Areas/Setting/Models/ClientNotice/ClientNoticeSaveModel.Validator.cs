using FluentValidation;

namespace shunda.Admin.Areas.Setting.Models.ClientNotice;

public class ClientNoticeSaveModelValidator : AbstractValidator<ClientNoticeSaveModel> {
    public ClientNoticeSaveModelValidator() { 
        RuleFor(e => e.Title).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("标题不能为空！");
        RuleFor(e => e.Title).MaximumLength(300).WithMessage("标题长度不能大于300！");

        RuleFor(e => e.Url).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("URL不能为空！");
        RuleFor(e => e.Url).MaximumLength(500).WithMessage("URL长度不能大于500！");
    }
}