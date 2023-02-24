using FluentValidation;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端公告 实体验证器
/// </summary>
public class ClientNoticeInfoValidator : AbstractValidator<ClientNoticeInfo> {
    public ClientNoticeInfoValidator() {
        RuleFor(e => e.Title).NotNull().WithMessage("标题不能为空！");
        RuleFor(e => e.Title).MaximumLength(300).WithMessage("标题长度不能大于300！");

        RuleFor(e => e.Url).NotNull().WithMessage("URL不能为空！");
        RuleFor(e => e.Url).MaximumLength(500).WithMessage("URL长度不能大于500！");

        RuleFor(e => e.EditorUser).Must(p => p != null).WithMessage("编辑者不能为空！");
    }
}