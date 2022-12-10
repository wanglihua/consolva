using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocSaveModel
{
    /// <summary>
    ///     StorageLocId 显示详情界面时，用于 HTTP 接收参数
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     货架
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    public int ShelfId { get; set; }

    public StorageLocInfo StorageLoc { get; set; } = new();

    /// <summary>
    ///     货架 Id， 用于界面显示和传值
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    public int SpecId { get; set; }
}