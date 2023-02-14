using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端内容阅读记录
/// </summary>
public class CustomerClientContentViewRecordInfo : Entity {
    /// <summary>
    /// 客户ID，非空
    /// </summary>
    public virtual int CustomerId { get; set; } = 0;

    /// <summary>
    /// 客户端内容类型，非空
    /// </summary>
    public virtual ClientContentType Type { get; set; } = ClientContentType.None;

    /// <summary>
    /// 关联ID，非空
    /// </summary>
    public virtual int RelatedId { get; set; } = 0;

    /// <summary>
    /// 最后阅读时间，非空
    /// </summary>
    public virtual DateTime LastReadTime { get; set; } = DateTime.MinValue;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CustomerClientContentViewRecordInfoValidator,  CustomerClientContentViewRecordInfo>(this);
    }
}