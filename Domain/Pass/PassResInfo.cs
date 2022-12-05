using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 授权资源
/// </summary>
public class PassResInfo : EntityNoCreateNoUpdate
{
    /// <summary>
    /// 类别
    /// </summary>
    [Required(ErrorMessage = "类别不能为空！")]
    public virtual PassResCateInfo Cate { get; set; } = new ();

    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(100, ErrorMessage = "名称长度不能大于100！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    public virtual string? Description { get; set; }
    
    /// <summary>
    /// 序号
    /// </summary>
    public virtual int OrderNum { get; set; }

    #region CacheData

    public virtual int CateIdForCache { get; set; }

    #endregion
}