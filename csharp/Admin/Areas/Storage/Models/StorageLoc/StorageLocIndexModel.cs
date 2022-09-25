using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocIndexModel
{
    [BindNever]
    public int ShelfId { get; set; }

    [BindNever]
    public ShelfInfo Shelf { get; set; } = new ShelfInfo();

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    // public IList<StorageLocDuplicateInfo> StorageDuplicateList { get; set; }

#if DEBUG
    public bool Debug = true;
#else
        public bool Debug = false;
#endif
}