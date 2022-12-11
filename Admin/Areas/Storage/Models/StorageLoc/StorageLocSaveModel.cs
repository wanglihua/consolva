using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocSaveModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 层
    /// </summary>
    [Required(ErrorMessage = "层不能为空！")]
    public int RowNum { get; set; }

    /// <summary>
    /// 列
    /// </summary>
    [Required(ErrorMessage = "列不能为空！")]
    public int ColNum { get; set; }

    /// <summary>
    /// 规格
    /// </summary>
    [Required(ErrorMessage = "规格不能为空！")]
    public int SpecId { get; set; }

    /// <summary>
    /// 货架
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    public int ShelfId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(StorageLocStatusModelBinder))]
    public StorageLocStatus Status { get; set; } = StorageLocStatus.None;

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空！")]
    public int OrderNum { get; set; }

    /// <summary>
    /// 快递单下架仓位顺序
    /// </summary>
    public int OffShelfOrderNum { get; set; }
}