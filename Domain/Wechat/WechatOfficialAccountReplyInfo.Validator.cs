using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号自动回复设置 实体验证器
/// </summary>
public class WechatOfficialAccountReplyInfoValidator : AbstractValidator<WechatOfficialAccountReplyInfo> {
    public WechatOfficialAccountReplyInfoValidator() {
        RuleFor(e => e.Type).NotNull().Must(p => p != WechatOfficialAccountReplyType.None).WithMessage("回复类型不能为空！");

        RuleFor(e => e.Content).NotNull().WithMessage("回复内容不能为空！");
        RuleFor(e => e.Content).MaximumLength(2000).WithMessage("回复内容长度不能大于2000！");
    }
}