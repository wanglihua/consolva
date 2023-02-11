using FluentValidation;

namespace shunda.Domain.Pass;

public class GroupInfoValidator : AbstractValidator<GroupInfo> {
    public GroupInfoValidator() {
        RuleFor(e => e.Code).NotNull().WithMessage("编码不能为空！");
        RuleFor(e => e.Code).MaximumLength(50).WithMessage("编码长度不能大于50！");

        RuleFor(e => e.Name).NotNull().WithMessage("名称不能为空！");
        RuleFor(e => e.Name).MaximumLength(50).WithMessage("名称长度不能大于50！");

        RuleFor(e => e.Description).MaximumLength(200).WithMessage("描述长度不能大于200！");
    }
}