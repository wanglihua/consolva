using FluentValidation;

namespace shunda.Service.Pay;

public class AlipayChargeCreateParam {
    public int CustomerId { get; set; }

    public decimal Amount { get; set; }

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

public class AlipayChargeCreateParamValidator : AbstractValidator<AlipayChargeCreateParam> {
    public AlipayChargeCreateParamValidator() {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户ID不能为空！");
        RuleFor(e => e.Amount).Must(p => p > 0).WithMessage("充值金额不能为空或小于0！");
        RuleFor(e => e.ReturnUrl).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("return url 不能为空！");
        RuleFor(e => e.NotifyUrl).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("notify url 不能为空！");
    }
}