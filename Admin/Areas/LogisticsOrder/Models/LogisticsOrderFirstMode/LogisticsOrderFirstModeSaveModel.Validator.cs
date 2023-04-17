using FluentValidation;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeSaveModelValidator : AbstractValidator<LogisticsOrderFirstModeSaveModel> {
    public LogisticsOrderFirstModeSaveModelValidator() {
        RuleFor(e => e.OrderFirstModeId).Must(p => p > 0).WithMessage("订单物流方式ID不能为空！");

        // RuleFor(e => e.TrackingNo).NotNull().WithMessage("快递单号不能为空！");
        RuleFor(e => e.TrackingNo).MaximumLength(30).WithMessage("快递单号长度不能大于30！");
    }
}