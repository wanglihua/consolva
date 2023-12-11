using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Express;

/// <summary>
/// 入库跟踪单号快递
/// </summary>
public class CheckinTrackingNoExpressInfo : Entity {
    /// <summary>
    /// 入库跟踪号，非空
    /// </summary>
    public virtual string CheckinTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递单id，非空
    /// </summary>
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

    /// <summary>
    /// 尺寸重量id，可空
    /// </summary>
    public virtual ExpressSizeWeightInfo? SizeWeight { get; set; } = null;

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual CheckinTrackingNoExpressStatus Status { get; set; } = CheckinTrackingNoExpressStatus.None;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CheckinTrackingNoExpressInfoValidator, CheckinTrackingNoExpressInfo>(this);
    }

    #region cache

    /// <summary>
    /// 排序用
    /// </summary>
    public virtual int SeqCache { get; set; }

    #endregion
}