using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace shunda.Domain.Pass;

/// <summary>
/// 资源类别
/// </summary>
public class PassResCateInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(100, ErrorMessage = "名称长度不能大于100！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [DisplayName("描述")]
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    public virtual int OrderNum { get; set; }
}