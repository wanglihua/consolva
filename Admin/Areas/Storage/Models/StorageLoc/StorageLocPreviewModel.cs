using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocPreviewModel
{
    [BindNever]
    public int ShelfId { get; set; }

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public ShelfInfo Shelf { get; set; } = new();

    [BindNever]
    public IList<StorageLocInfo> StorageLocList { get; set; } = new List<StorageLocInfo>();

    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();
}