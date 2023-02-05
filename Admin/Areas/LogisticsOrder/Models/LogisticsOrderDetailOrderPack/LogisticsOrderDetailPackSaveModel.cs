using FluentValidation;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;

public class LogisticsOrderDetailOrderPackSavelModel {
    public int OrderPackId { get; set; }

    public decimal Width { get; set; }

    public decimal Length { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }
}

public class LogisticsOrderDetailOrderPackSavelModelValidator : AbstractValidator<LogisticsOrderDetailOrderPackSavelModel> {
    public LogisticsOrderDetailOrderPackSavelModelValidator() {
        RuleFor(e => e.OrderPackId).Must(p => p > 0).WithMessage("订单打包Id不能为空！");
        RuleFor(e => e.Width).Must(p => p > 0).WithMessage("宽度不能为空！");
        RuleFor(e => e.Length).Must(p => p > 0).WithMessage("长度不能为空！");
        RuleFor(e => e.Height).Must(p => p > 0).WithMessage("高度不能为空！");
        RuleFor(e => e.Weight).Must(p => p > 0).WithMessage("重量不能为空！");
    }
}