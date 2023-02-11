using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信场景 实体验证器
/// </summary>
public class WechatSceneInfoValidator : AbstractValidator<WechatSceneInfo> {
    public WechatSceneInfoValidator() { 
        RuleFor(e => e.SceneId).Must(p => p > 0).WithMessage("场景Id不能为空！");

        RuleFor(e => e.Type).NotNull().Must(p => p != WechatSceneType.None).WithMessage("关联场景类型不能为空！");

        RuleFor(e => e.RelatedId).Must(p => p > 0).WithMessage("关联Id不能为空！");

    }
}