using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfIndexModel
{
    [BindNever]
    public int WarehouseId { get; set; }

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    [BindNever]
    public IList<ShelfType> ShelfTypeList { get; set; } = new List<ShelfType>();

    // public IList<ShelfDuplicateInfo> ShelfDuplicateList { get; set; }

    // public IList<StorageLocDuplicateInfo> StorageDuplicateList { get; set; }
}