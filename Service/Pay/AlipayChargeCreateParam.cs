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