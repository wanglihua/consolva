using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerChargeAwardSetting;

public class CustomerChargeAwardSettingSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int CustomerChargeAwardSettingId { get; set; }

    /// <summary>
    /// 最小充值金额，非空
    /// </summary>
    public int AmountBegin { get; set; } = 0;

    /// <summary>
    /// 最大充值金额，非空
    /// </summary>
    public int AmountEnd { get; set; } = 0;

    /// <summary>
    /// 奖励金额，非空
    /// </summary>
    public int Award { get; set; } = 0;

}