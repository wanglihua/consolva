using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfSaveModel
{
    public int Id { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(50, ErrorMessage = "编码长度不能大于50！")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 货架名
    /// </summary>
    [Required(ErrorMessage = "货架名不能为空！")]
    [MaxLength(255, ErrorMessage = "货架名长度不能大于255！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 货架类型
    /// </summary>
    [Required(ErrorMessage = "货架类型不能为空！")]
    [ModelBinder(typeof(ShelfTypeModelBinder))]
    public ShelfType Type { get; set; } = ShelfType.None;

    /// <summary>
    /// 货架类别
    /// </summary>
    [Required(ErrorMessage = "货架类别不能为空！")]
    [ModelBinder(typeof(ShelfCateModelBinder))]
    public ShelfCate Cate { get; set; } = ShelfCate.None;

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(ShelfStatusModelBinder))]
    public ShelfStatus Status { get; set; } = ShelfStatus.None;

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空！")]
    public int OrderNum { get; set; }

    /// <summary>
    /// 仓库
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    public int WarehouseId { get; set; }
}