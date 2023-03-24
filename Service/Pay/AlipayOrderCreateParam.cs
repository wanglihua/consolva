using FluentValidation;

namespace shunda.Service.Pay;

public class AlipayOrderCreateParam {
    public int OrderId { get; set; }

    public int CouponId { get; set; }

    public bool ScoreDeduct { get; set; }

    public bool IsMobile { get; set; }

    /// <summary>
    /// 同步回调URL，URL中已经带有 域名等信息
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// 异步回调URL，URL中已经带有 域名等信息
    /// </summary>
    public string NotifyUrl { get; set; } = string.Empty;
}

public class AlipayOrderCreateParamValidator : AbstractValidator<AlipayOrderCreateParam> {
    public AlipayOrderCreateParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单ID不能为空！");
        RuleFor(e => e.ReturnUrl).NotNull().NotEmpty().WithMessage("return url 不能为空！");
        RuleFor(e => e.NotifyUrl).NotNull().NotEmpty().WithMessage("notify url 不能为空！");
    }
}