using FluentValidation;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntrySaveModelValidator : AbstractValidator<LogisticsFeeRateExpressEntrySaveModel> {
    public LogisticsFeeRateExpressEntrySaveModelValidator() { 
        RuleFor(e => e.FeeRateExpressId).Must(p => p > 0).WithMessage("快递具体费率不能为空！");

        RuleFor(e => e.BeginWeight).Must(p => p > 0).WithMessage("开始重量不能为空！");

        RuleFor(e => e.EndWeight).Must(p => p > 0).WithMessage("结束重量不能为空！");

        RuleFor(e => e.FeeByKilogram).NotNull().NotEmpty().WithMessage("是否按公斤计费不能为空！");

    }
}