using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号消息 实体验证器
/// </summary>
public class WechatOfficialAccountMessageInfoValidator : AbstractValidator<WechatOfficialAccountMessageInfo>
{
    public WechatOfficialAccountMessageInfoValidator()
    {
        RuleFor(e => e.Direction).NotNull().WithMessage("消息方向不能为空！");
        RuleFor(e => e.Direction).Must(p => p != WechatOfficialAccountMessageDirection.None).WithMessage("消息方向不能为空！");

        RuleFor(e => e.OpenId).NotNull().WithMessage("OpenId不能为空！");
        RuleFor(e => e.OpenId).MaximumLength(100).WithMessage("OpenId长度不能大于100！");

        RuleFor(e => e.Message).NotNull().WithMessage("消息不能为空！");
        RuleFor(e => e.Message).MaximumLength(800).WithMessage("消息长度不能大于800！");

        RuleFor(e => e.Status).NotNull().WithMessage("状态不能为空！");
        RuleFor(e => e.Status).Must(p => p != WechatOfficialAccountMessageStatus.None).WithMessage("状态不能为空！");
    }
}