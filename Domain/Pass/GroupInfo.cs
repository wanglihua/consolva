using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Pass;

/// <summary>
/// 群组
/// </summary>
public class GroupInfo : Entity {
    /// <summary>
    /// 编码
    /// </summary>
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public virtual string? Description { get; set; }

    public override void Validate() {
        ValidationUtil.ValidateEntity<GroupInfoValidator, GroupInfo>(this);
    }
} 