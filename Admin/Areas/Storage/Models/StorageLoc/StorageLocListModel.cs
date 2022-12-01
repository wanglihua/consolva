using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocListModel : ListModel
{
    public StorageLocListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<StorageLocInfo> StorageLocList { get; set; } = new List<StorageLocInfo>();

    [BindNever]
    public string StorageLocSpecAlias { get; set; } = string.Empty;
}