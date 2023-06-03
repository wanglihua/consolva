using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户充值优惠设置
/// </summary>
public class CustomerChargeAwardSettingInfo : Entity {
    /// <summary>
    /// 最小充值金额，非空
    /// </summary>
    public virtual int AmountBegin { get; set; } = 0;

    /// <summary>
    /// 最大充值金额，非空
    /// </summary>
    public virtual int AmountEnd { get; set; } = 0;

    /// <summary>
    /// 奖励金额，非空
    /// </summary>
    public virtual int Award { get; set; } = 0;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CustomerChargeAwardSettingInfoValidator, CustomerChargeAwardSettingInfo>(this);
    }
}