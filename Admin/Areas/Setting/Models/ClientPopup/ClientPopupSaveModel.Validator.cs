using FluentValidation;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientPopup;

public class ClientPopupSaveModelValidator : AbstractValidator<ClientPopupSaveModel> {
    public ClientPopupSaveModelValidator() { 
        RuleFor(e => e.ShouldPopup).NotNull().WithMessage("是否弹窗不能为空！");

        RuleFor(e => e.Content).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("内容不能为空！");
        RuleFor(e => e.Content).MaximumLength(1000).WithMessage("内容长度不能大于1000！");
    }
}