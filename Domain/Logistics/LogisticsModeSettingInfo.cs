using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流配置
/// </summary>
public class LogisticsModeSettingInfo : Entity {
    /// <summary>
    /// 默认体积重分母，非空
    /// </summary>
    public virtual int VolumeWeightDenominator { get; set; } = 0;

    /// <summary>
    /// 是否有免费包裹数量限制，非空
    /// </summary>
    public virtual bool HasFreePackageLimit { get; set; } = false;

    /// <summary>
    /// 免费包裹数量限制，可空
    /// </summary>
    public virtual int? FreePackageLimit { get; set; } = null;

    /// <summary>
    /// 额外包裹收费，可空
    /// </summary>
    public virtual decimal? ExtraPackageFee { get; set; } = null;

    /// <summary>
    /// 是否有额外包裹收费封顶金额，非空
    /// </summary>
    public virtual bool HasExtraPackageFeeCapAmount { get; set; } = false;

    /// <summary>
    /// 额外包裹收费封顶金额，可空
    /// </summary>
    public virtual decimal? ExtraPackageFeeCapAmount { get; set; } = null;

    public override void Validate() {
        ValidationUtil.ValidateEntity<LogisticsModeSettingInfoValidator, LogisticsModeSettingInfo>(this);
    }
}