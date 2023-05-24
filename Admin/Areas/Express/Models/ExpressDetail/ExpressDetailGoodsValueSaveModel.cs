using FluentValidation;

namespace shunda.Admin.Areas.Express.Models.ExpressDetail;

public class ExpressDetailGoodsValueSaveModel {
    /// <summary>
    /// 快递单号Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 货品总值
    /// </summary>
    public decimal GoodsValue { get; set; }
}

public class ExpressDetailGoodsValueSaveModelValidator : AbstractValidator<ExpressDetailGoodsValueSaveModel> {
    public ExpressDetailGoodsValueSaveModelValidator() {
        RuleFor(e => e.Id).Must(p => p > 0).WithMessage("快递单Id不能为空！");
        RuleFor(e => e.GoodsValue).Must(p => p > 0).WithMessage("货品总值不能为空！");
    }
}