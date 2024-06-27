using FluentValidation;
using shunda.Domain.Express;

namespace shunda.Domain.Message;

/// <summary>
/// 快递图片消息发送时间 实体验证器
/// </summary>
public class ExpressPhotoMessageSendTimeInfoValidator : AbstractValidator<ExpressPhotoMessageSendTimeInfo>
{
    public ExpressPhotoMessageSendTimeInfoValidator()
    {
        RuleFor(e => e.TrackingNo).MaximumLength(30).WithMessage("快递单号长度不能大于30！");

        RuleFor(e => e.Type).NotNull().WithMessage("图片类型不能为空！");
        RuleFor(e => e.Type).Must(p => p != ExpressPhotoType.None).WithMessage("图片类型不能为空！");
    }
}