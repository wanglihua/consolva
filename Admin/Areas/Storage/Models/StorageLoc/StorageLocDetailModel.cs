using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocDetailModel : StorageLocSaveModel
{
    /// <summary>
    /// 名称，这个名称只做展示，他的值是生成的
    /// </summary>
    // [Required(ErrorMessage = "名称不能为空！")]
    // [MaxLength(255, ErrorMessage = "名称长度不能大于255！")]
    public string Name { get; set; } = String.Empty;
    
    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();

    [BindNever]
    public IList<StorageLocStatus> StorageLocStatusList { get; set; } = new List<StorageLocStatus>();
}