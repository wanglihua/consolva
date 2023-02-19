using FluentValidation;

namespace shunda.Admin.Areas.Pass.Models.Group;

public class GroupSaveModelValidator : AbstractValidator<GroupSaveModel> {
    public GroupSaveModelValidator() {
        RuleFor(e => e.Code).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("编码不能为空！");
        RuleFor(e => e.Code).MaximumLength(50).WithMessage("编码长度不能大于50！");

        RuleFor(e => e.Name).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("名称不能为空！");
        RuleFor(e => e.Name).MaximumLength(50).WithMessage("名称长度不能大于50！");

        RuleFor(e => e.Description).MaximumLength(200).WithMessage("描述长度不能大于200！");
    }
} 