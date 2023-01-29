using FluentValidation;

namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderCancelParam {
    public int OrderId { get; set; }

    /// <summary>
    /// 是不是后台取消订单
    /// </summary>
    public bool Backend { get; set; }

    public int CustomerId { get; set; }

    public int UserId { get; set; }
}

public class LogisticsOrderCancelParamValidator : AbstractValidator<LogisticsOrderCancelParam> {
    public LogisticsOrderCancelParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单Id不能为空！");
        RuleFor(e => e.CustomerId).Must(p => p > 0).When(e => e.Backend == false).WithMessage("客户端取消订单，客户ID不能为空！");
        RuleFor(e => e.UserId).Must(p => p > 0).When(e => e.Backend == true).WithMessage("后台取消订单，用户ID不能为空！");
    }
}