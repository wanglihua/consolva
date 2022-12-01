using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLocSpec;

public class StorageLocSpecIndexModel
{
    [BindNever]
    public IList<ShelfType> TypeList { get; set; } = new List<ShelfType>();
}