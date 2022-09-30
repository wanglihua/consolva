using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 群组
/// </summary>
public class GroupInfo : Entity
{
    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(50, ErrorMessage = "编码长度不能大于50！")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>

    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50, ErrorMessage = "名称长度不能大于50！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(200, ErrorMessage = "描述长度不能大于200！")]
    public virtual string? Description { get; set; }
}