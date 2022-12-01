using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLocSpec;

public class StorageLocSpecDetailModel
{
    [BindNever]
    public IList<ShelfType> TypeList { get; set; } = new List<ShelfType>();

    [BindNever]
    public IList<WebColor> ColorList { get; set; } = new List<WebColor>();

    public StorageLocSpecInfo StorageLocSpec { get; set; } = new();
}