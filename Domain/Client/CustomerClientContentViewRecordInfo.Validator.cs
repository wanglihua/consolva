using FluentValidation;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端内容阅读记录 实体验证器
/// </summary>
public class CustomerClientContentViewRecordInfoValidator : AbstractValidator<CustomerClientContentViewRecordInfo>
{
    public CustomerClientContentViewRecordInfoValidator()
    {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户ID不能为空！");

        RuleFor(e => e.Type).NotNull().WithMessage("客户端内容类型不能为空！");
        RuleFor(e => e.Type).Must(p => p != ClientContentType.None).WithMessage("客户端内容类型不能为空！");

        RuleFor(e => e.RelatedId).Must(p => p >= 0).WithMessage("关联ID不能为空！");

        RuleFor(e => e.LastReadTime).NotNull().WithMessage("最后阅读时间不能为空！");
    }
}