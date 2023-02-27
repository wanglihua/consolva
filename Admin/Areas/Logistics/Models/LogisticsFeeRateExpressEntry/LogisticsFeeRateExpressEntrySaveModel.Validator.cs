using FluentValidation;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntrySaveModelValidator : AbstractValidator<LogisticsFeeRateExpressEntrySaveModel> {
    public LogisticsFeeRateExpressEntrySaveModelValidator() {
        RuleFor(e => e.FeeRateExpressId).Must(p => p > 0).WithMessage("快递具体费率不能为空！");

        RuleFor(e => e.BeginWeight).Must(p => p >= 0).WithMessage("开始重量不能为为负数！");
        RuleFor(e => e.EndWeight).Must(p => p > 0).WithMessage("结束重量必须为正数！");
        RuleFor(e => e.EndWeight).GreaterThan(e => e.BeginWeight).WithMessage("结束重量必须大于开始重量！");

        RuleFor(e => e.FeePerKilogram).Must(p => p > 0).WithMessage("每公斤费率不能为空！");
    }
}