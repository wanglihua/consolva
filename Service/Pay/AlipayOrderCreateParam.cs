namespace shunda.Service.Pay;

public class AlipayOrderCreateParam {
    public int OrderId { get; set; }

    public int CouponId { get; set; }

    public bool IsMobile { get; set; }

    /// <summary>
    /// 同步回调URL，URL中已经带有 域名等信息
    /// </summary>
    public string CallbackUrl { get; set; } = string.Empty;

    /// <summary>
    /// 异步回调URL，URL中已经带有 域名等信息
    /// </summary>
    public string NotifyUrl { get; set; } = string.Empty;
}