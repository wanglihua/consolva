using shunda.Core;
using shunda.Core.Util;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Message;

/// <summary>
/// 快递图片消息发送时间
/// </summary>
public class ExpressPhotoMessageSendTimeInfo : Entity {
    /// <summary>
    /// 快递单号，可空
    /// </summary>
    public virtual string? TrackingNo { get; set; } = null;

    /// <summary>
    /// 快递单，可空
    /// </summary>
    public virtual ExpressInfo? Express { get; set; } = null;

    /// <summary>
    /// 订单，可空
    /// </summary>
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 图片类型，非空
    /// </summary>
    public virtual ExpressPhotoType Type { get; set; } = ExpressPhotoType.None;

    /// <summary>
    /// 发送时间，非空
    /// </summary>
    public virtual DateTime LastSendTime { get; set; } = DateTime.MinValue;

    public override void Validate() {
        ValidationUtil.ValidateEntity<ExpressPhotoMessageSendTimeInfoValidator, ExpressPhotoMessageSendTimeInfo>(this);
    }
}