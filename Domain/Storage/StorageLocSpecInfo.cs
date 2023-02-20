using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Storage;

/// <summary>
/// 库位标准
/// </summary>
public class StorageLocSpecInfo : Entity {
    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(ShelfTypeModelBinder))]
    public virtual ShelfType ShelfType { get; set; } = ShelfType.None;

    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    // [MaxLength(50, ErrorMessage = "编码长度不能大于50！")]
    [MaxLength(1, ErrorMessage = "编码长度不能大于1！")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    // [Required(ErrorMessage = "名称不能为空！")]，可以是空字符串
    [MaxLength(255, ErrorMessage = "名称长度不能大于255！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 宽/cm
    /// </summary>
    [Required(ErrorMessage = "宽/cm不能为空！")]
    public virtual int Width { get; set; }

    /// <summary>
    /// 高/cm
    /// </summary>
    [Required(ErrorMessage = "高/cm不能为空！")]
    public virtual int Height { get; set; }

    /// <summary>
    /// 长/cm
    /// </summary>
    [Required(ErrorMessage = "长/cm不能为空！")]
    public virtual int Length { get; set; }

    /// <summary>
    /// 颜色
    /// </summary>
    [Required(ErrorMessage = "颜色不能为空！")]
    public virtual string Color { get; set; } = string.Empty;

    /// <summary>
    /// 序号
    /// </summary>
    [Required(ErrorMessage = "序号不能为空！")]
    public virtual int OrderNum { get; set; }
}