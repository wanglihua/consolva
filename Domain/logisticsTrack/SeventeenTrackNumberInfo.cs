using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK注册快递单
/// </summary>
public class SeventeenTrackNumberInfo : Entity {
    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 服务商代码，非空
    /// </summary>
    public virtual int CarrierKey { get; set; } = 0;

    /// <summary>
    /// 服务商名称，非空
    /// </summary>
    public virtual string CarrierName { get; set; } = string.Empty;

    /// <summary>
    /// 运输商识别，非空
    /// </summary>
    public virtual SeventeenTrackNumberOrigin Origin { get; set; } = SeventeenTrackNumberOrigin.None;

    /// <summary>
    /// 错误代码，非空
    /// </summary>
    public virtual int ErrorCode { get; set; } = 0;

    /// <summary>
    /// 错误描述，非空
    /// </summary>
    public virtual string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual SeventeenTrackNumberStatus Status { get; set; } = SeventeenTrackNumberStatus.None;

    /// <summary>
    /// 跟踪状态
    /// </summary>
    public virtual string TrackStatus { get; set; } = string.Empty;

    /// <summary>
    /// 跟踪子状态
    /// </summary>
    public virtual string TrackSubStatus { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<SeventeenTrackNumberInfoValidator, SeventeenTrackNumberInfo>(this);
    }
}