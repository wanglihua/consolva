using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户推广
/// </summary>
public class CustomerPromotionInfo : Entity {
    /// <summary>
    /// 父id，非空
    /// </summary>
    public virtual CustomerInfo Parent { get; set; } = new CustomerInfo();

    /// <summary>
    /// 子id，可空
    /// </summary>
    public virtual CustomerInfo? Child { get; set; } = null;

    /// <summary>
    /// open id，可空
    /// </summary>
    public virtual string? OpenId { get; set; } = null;

    /// <summary>
    /// 下家消费金额，可空
    /// </summary>
    public virtual decimal? ChildPayAmount { get; set; } = null;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CustomerPromotionInfoValidator, CustomerPromotionInfo>(this);
    }
}