using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocDetailModel : StorageLocSaveModel
{
    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();

    [BindNever]
    public IList<StorageLocStatus> StorageLocStatusList { get; set; } = new List<StorageLocStatus>();
}