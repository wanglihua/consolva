using System.ComponentModel;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 群组资源授权
/// </summary>
public class GroupPassResInfo : EntityNoUpdate
{
    /// <summary>
    /// 群组
    /// </summary>
    public virtual GroupInfo Group { get; set; } = new ();

    /// <summary>
    /// 资源
    /// </summary>
    public virtual PassResInfo PassRes { get; set; } = new ();
}