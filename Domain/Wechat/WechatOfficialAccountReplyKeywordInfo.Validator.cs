using FluentValidation;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号关键字回复设置 实体验证器
/// </summary>
public class WechatOfficialAccountReplyKeywordInfoValidator : AbstractValidator<WechatOfficialAccountReplyKeywordInfo> {
    public WechatOfficialAccountReplyKeywordInfoValidator() { 
        RuleFor(e => e.Keyword).NotNull().WithMessage("关键字不能为空！");
        RuleFor(e => e.Keyword).MaximumLength(200).WithMessage("关键字长度不能大于200！");

        RuleFor(e => e.Content).NotNull().WithMessage("回复内容不能为空！");
        RuleFor(e => e.Content).MaximumLength(2000).WithMessage("回复内容长度不能大于2000！");

    }
