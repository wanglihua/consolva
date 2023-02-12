using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号OAuth专用AccessToken 实体验证器
/// </summary>
public class WechatOfficialAccountOAuthAccessTokenInfoValidator : AbstractValidator<WechatOfficialAccountOAuthAccessTokenInfo> {
    public WechatOfficialAccountOAuthAccessTokenInfoValidator() { 
        RuleFor(e => e.Code).NotNull().WithMessage("code不能为空！");
        RuleFor(e => e.Code).MaximumLength(50).WithMessage("code长度不能大于50！");

        RuleFor(e => e.AccessToken).NotNull().WithMessage("access token不能为空！");
        RuleFor(e => e.AccessToken).MaximumLength(600).WithMessage("access token长度不能大于600！");

        RuleFor(e => e.OpenId).NotNull().WithMessage("open id不能为空！");
        RuleFor(e => e.OpenId).MaximumLength(50).WithMessage("open id长度不能大于50！");

    }
}