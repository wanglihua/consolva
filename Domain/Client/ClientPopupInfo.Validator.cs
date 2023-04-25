using FluentValidation;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端弹窗 实体验证器
/// </summary>
public class ClientPopupInfoValidator : AbstractValidator<ClientPopupInfo> {
    public ClientPopupInfoValidator() { 
        RuleFor(e => e.Content).NotNull().WithMessage("内容不能为空！");
        RuleFor(e => e.Content).MaximumLength(1000).WithMessage("内容长度不能大于1000！");
    }
}