namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;

public class LogisticsModeSettingSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int LogisticsModeSettingId { get; set; }

    /// <summary>
    /// 默认体积重分母，非空
    /// </summary>
    public int VolumeWeightDenominator { get; set; } = 0;

    /// <summary>
    /// 是否有免费包裹数量限制，非空
    /// </summary>
    public bool HasFreePackageLimit { get; set; } = false;

    /// <summary>
    /// 免费包裹数量限制，可空
    /// </summary>
    public int? FreePackageLimit { get; set; } = null;

    /// <summary>
    /// 额外包裹收费，可空
    /// </summary>
    public decimal? ExtraPackageFee { get; set; } = null;

    /// <summary>
    /// 是否有额外包裹收费封顶金额，非空
    /// </summary>
    public bool HasExtraPackageFeeCapAmount { get; set; } = false;

    /// <summary>
    /// 额外包裹收费封顶金额，可空
    /// </summary>
    public decimal? ExtraPackageFeeCapAmount { get; set; } = null;

}