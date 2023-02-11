using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号关注者 实体验证器
/// </summary>
public class WechatOfficialAccountFollowerInfoValidator : AbstractValidator<WechatOfficialAccountFollowerInfo> {
    public WechatOfficialAccountFollowerInfoValidator() { 

        RuleFor(e => e.NickName).MaximumLength(100).WithMessage("微信昵称长度不能大于100！");

        RuleFor(e => e.OpenId).NotNull().NotEmpty().WithMessage("open id不能为空！");
        RuleFor(e => e.OpenId).MaximumLength(100).WithMessage("open id长度不能大于100！");

        RuleFor(e => e.FollowStatus).NotNull().Must(p => p != WechatOfficialAccountFollowStatus.None).WithMessage("关注状态不能为空！");



    }
}