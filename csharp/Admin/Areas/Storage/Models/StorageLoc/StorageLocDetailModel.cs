using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocDetailModel
{
    /// <summary>
    /// StorageLocId 显示详情界面时，用于 HTTP 接收参数
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 货架
    /// </summary>
    [DisplayName("货架")]
    [Required(ErrorMessage = "货架不能为空！")]
    public int ShelfId { get; set; }

    public StorageLocInfo StorageLoc { get; set; } = new StorageLocInfo();

    /// <summary>
    /// 货架 Id， 用于界面显示和传值
    /// </summary>
    [DisplayName("货架")]
    [Required(ErrorMessage = "货架不能为空！")]
    public int SpecId { get; set; }

    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();

    [BindNever]
    public IList<StorageLocStatus> StorageLocStatusList { get; set; } = new List<StorageLocStatus>();
}