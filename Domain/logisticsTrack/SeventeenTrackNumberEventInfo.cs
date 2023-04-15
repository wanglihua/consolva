using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK快递单事件
/// </summary>
public class SeventeenTrackNumberEventInfo : Entity {
    /// <summary>
    /// 17TRACK注册快递单，非空
    /// </summary>
    public virtual SeventeenTrackNumberInfo TrackNumber { get; set; } = new SeventeenTrackNumberInfo();

    /// <summary>
    /// 时间，非空
    /// </summary>
    public virtual DateTime EventTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 描述，非空
    /// </summary>
    public virtual string EventDesc { get; set; } = string.Empty;

    /// <summary>
    /// 里程碑状态，非空
    /// </summary>
    public virtual string Stage { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<SeventeenTrackNumberEventInfoValidator, SeventeenTrackNumberEventInfo>(this);
    }
}