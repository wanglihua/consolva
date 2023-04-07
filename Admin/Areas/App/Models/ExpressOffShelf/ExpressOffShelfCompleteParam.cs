using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.App.Models.ExpressOffShelf;

public class ExpressOffShelfCompleteParam {
    [BindProperty(Name = "shelfId")]
    public int ShelfId { get; set; }

    [BindProperty(Name = "trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;
}

public class ExpressOffShelfCompleteParamValidator : AbstractValidator<ExpressOffShelfCompleteParam> {
    public ExpressOffShelfCompleteParamValidator() {
        RuleFor(e => e.ShelfId).Must(p => p > 0).WithMessage("货架不能为空！");
        RuleFor(e => e.TrackingNo).NotNull().NotEmpty().WithMessage("快递单号或入库跟踪号不能为空！");
    }
}