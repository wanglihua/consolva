using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;

namespace shunda.Domain.Warehouse;

/// <summary>
/// 货架
/// </summary>
public class ShelfInfo : Entity
{
    /// <summary>
    /// 编码
    /// </summary>
    [DisplayName("编码")]
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(50, ErrorMessage = "编码长度不能大于50！")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 货架名
    /// </summary>
    [DisplayName("货架名")]
    [Required(ErrorMessage = "货架名不能为空！")]
    [MaxLength(255, ErrorMessage = "货架名长度不能大于255！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 货架类型
    /// </summary>
    [DisplayName("货架类型")]
    [Required(ErrorMessage = "货架类型不能为空！")]
    [ModelBinder(typeof(ShelfTypeModelBinder))]
    public virtual ShelfType Type { get; set; } = ShelfType.None;

    /// <summary>
    /// 货架类别
    /// </summary>
    [DisplayName("货架类别")]
    [Required(ErrorMessage = "货架类别不能为空！")]
    [ModelBinder(typeof(ShelfCateModelBinder))]
    public virtual ShelfCate Cate { get; set; } = ShelfCate.None;

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(ShelfStatusModelBinder))]
    public virtual ShelfStatus Status { get; set; } = ShelfStatus.None;

    /// <summary>
    /// 排序
    /// </summary>
    [DisplayName("排序")]
    [Required(ErrorMessage = "排序不能为空！")]
    public virtual int OrderNum { get; set; }

    /// <summary>
    /// 仓库
    /// </summary>
    [DisplayName("仓库")]
    // [Required(ErrorMessage = "仓库不能为空！")]
    [BindNever]
    public virtual WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();
}