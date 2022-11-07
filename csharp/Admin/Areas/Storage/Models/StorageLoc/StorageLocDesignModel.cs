using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocDesignModel
{
    public int ShelfId { get; set; }

    public string DesignContent { get; set; } = string.Empty;

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public ShelfInfo Shelf { get; set; } = new();
}