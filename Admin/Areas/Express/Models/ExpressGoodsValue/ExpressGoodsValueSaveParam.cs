using FluentValidation;

namespace shunda.Admin.Areas.Express.Models.ExpressGoodsValue;

public class ExpressGoodsValueSaveParam {
    /// <summary>
    /// 快递单号Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 货品总值
    /// </summary>
    public decimal ExpressGoodsValue { get; set; }
}

public class ExpressDetailGoodsValueSaveParamValidator : AbstractValidator<ExpressGoodsValueSaveParam> {
    public ExpressDetailGoodsValueSaveParamValidator() {
        RuleFor(e => e.Id).Must(p => p > 0).WithMessage("快递单Id不能为空！");
        RuleFor(e => e.ExpressGoodsValue).Must(p => p > 0).WithMessage("货品总值不能为空！");
    }
}