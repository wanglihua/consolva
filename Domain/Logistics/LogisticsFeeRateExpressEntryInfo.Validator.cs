using FluentValidation;

namespace shunda.Domain.Logistics;

/// <summary>
/// 快递具体费率明细 实体验证器
/// </summary>
public class LogisticsFeeRateExpressEntryInfoValidator : AbstractValidator<LogisticsFeeRateExpressEntryInfo> {
    public LogisticsFeeRateExpressEntryInfoValidator() {
        RuleFor(e => e.FeeRateExpress).Must(p => p != null).WithMessage("快递具体费率不能为空！");

        RuleFor(e => e.BeginWeight).Must(p => p > 0).WithMessage("开始重量不能为空！");

        RuleFor(e => e.EndWeight).Must(p => p > 0).WithMessage("结束重量不能为空！");

        RuleFor(e => e.FeePerKilogram).Must(p => p > 0).WithMessage("每公斤费率不能为空！");
    }
}