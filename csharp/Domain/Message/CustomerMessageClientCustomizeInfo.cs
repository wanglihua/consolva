using shunda.Core;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息定制
/// </summary>
public partial class CustomerMessageClientCustomizeInfo : Entity
{
    /// <summary>
    /// 客户，非空
    /// </summary>
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 消息类型，非空
    /// </summary>
    public virtual CustomerMessageType MessageType { get; set; } = CustomerMessageType.None;

    /// <summary>
    /// 消息是否发送，非空
    /// </summary>
    public virtual bool Send { get; set; }
}