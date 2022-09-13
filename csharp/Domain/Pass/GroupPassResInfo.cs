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
    [DisplayName("群组")]
    public virtual GroupInfo Group { get; set; } = new GroupInfo();

    /// <summary>
    /// 资源
    /// </summary>
    [DisplayName("资源")]
    public virtual PassResInfo PassRes { get; set; } = new PassResInfo();
}