using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;

namespace shunda.Domain.Storage;

/// <summary>
/// 库位
/// </summary>
public class StorageLocInfo : Entity
{
    /// <summary>
    /// 名称
    /// </summary>
    [DisplayName("名称")]
    // [Required(ErrorMessage = "名称不能为空！")]
    // [MaxLength(255, ErrorMessage = "名称长度不能大于255！")]
    public virtual string Name { get; set; } = String.Empty;

    /// <summary>
    /// 层
    /// </summary>
    [DisplayName("层")]
    [Required(ErrorMessage = "层不能为空！")]
    public virtual int RowNum { get; set; }

    /// <summary>
    /// 列
    /// </summary>
    [DisplayName("列")]
    [Required(ErrorMessage = "列不能为空！")]
    public virtual int ColNum { get; set; }

    /// <summary>
    /// 规格
    /// </summary>
    // [DisplayName("规格")]
    // [Required(ErrorMessage = "规格不能为空！")]
    [BindNever]
    public virtual StorageLocSpecInfo Spec { get; set; } = new StorageLocSpecInfo();

    /// <summary>
    /// 货架
    /// </summary>
    [DisplayName("货架")]
    [BindNever]
    public virtual ShelfInfo Shelf { get; set; } = new ShelfInfo();

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(StorageLocStatusModelBinder))]
    public virtual StorageLocStatus Status { get; set; } = StorageLocStatus.None;

    /// <summary>
    /// 排序
    /// </summary>
    [DisplayName("排序")]
    [Required(ErrorMessage = "排序不能为空！")]
    public virtual int OrderNum { get; set; }

    /// <summary>
    /// 快递单下架仓位顺序
    /// </summary>
    public virtual int OffShelfOrderNum { get; set; }

    #region Cache

    public virtual string GetFullName()
    {
        if (Shelf.Type != ShelfType.LongNoPickDump && Shelf.Type != ShelfType.LongNoProcessDump)
        {
            if (string.IsNullOrWhiteSpace(Spec?.Name))
            {
                return $"{Shelf.Name}-{RowNum}层-{ColNum}号";
            }
            else
            {
                // E排（货架的名字）-1（层数）层--9（列数）号-高层三仓位（规格）
                return $"{Shelf.Name}-{RowNum}层-{ColNum}号-{Spec.Name}";
            }
        }
        else
        {
            return $"{Shelf.Name}-{Name}";
        }
    }

    /// <summary>
    /// 获取仓位的货架名称
    /// </summary>
    /// <returns></returns>
    public virtual string GetShelfName()
    {
        return Shelf?.Name ?? "";
    }

    /// <summary>
    /// 获取只有行列的名称
    /// </summary>
    /// <returns></returns>
    public virtual string GetRowColName()
    {
        return $"{RowNum}层-{ColNum}号";
    }

    /// <summary>
    /// 用于业务计算的时候的暂时记录和排序用
    /// </summary>
    public virtual int PackageCountCache { get; set; }

    /// <summary>
    ///  只用于界面显示
    /// </summary>
    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    /// <summary>
    ///  只用于界面显示
    /// </summary>
    public virtual string OrderCodeCache { get; set; } = string.Empty;

    /// <summary>
    ///  只用于界面显示
    /// </summary>
    public virtual string TrackingNoCache { get; set; } = string.Empty;

    public virtual bool HasGoodsCache { get; set; }

    public virtual bool HasQueryGoodsCache { get; set; }

    public virtual string GoodsTypeCache { get; set; } = string.Empty;

    /// <summary>
    /// 仓位上快递单号数量
    /// </summary>
    public virtual int ExpressCountCache { get; set; }

    #endregion
}