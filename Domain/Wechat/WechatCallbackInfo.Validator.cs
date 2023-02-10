using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号回调记录 实体验证器
/// </summary>
public class WechatCallbackInfoValidator : AbstractValidator<WechatCallbackInfo> {
    public WechatCallbackInfoValidator() { 
        RuleFor(e => e.SystemType).NotNull().Must(p => p != WechatSystemType.None).WithMessage("微信系统类型不能为空！");

        RuleFor(e => e.Method).NotNull().Must(p => p != WechatCallbackHttpMethod.None).WithMessage("回调http方法不能为空！");

        RuleFor(e => e.Params).NotNull().NotEmpty().WithMessage("参数不能为空！");
        RuleFor(e => e.Params).MaximumLength(2000).WithMessage("参数长度不能大于2000！");

        RuleFor(e => e.RequestBody).NotNull().NotEmpty().WithMessage("body不能为空！");
        RuleFor(e => e.RequestBody).MaximumLength(2000).WithMessage("body长度不能大于2000！");

        RuleFor(e => e.Result).NotNull().NotEmpty().WithMessage("结果值不能为空！");
        RuleFor(e => e.Result).MaximumLength(2000).WithMessage("结果值长度不能大于2000！");

    }
}