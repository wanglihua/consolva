using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLocSpec;

public class StorageLocSpecListModel : ListModel
{
    public StorageLocSpecListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();
}