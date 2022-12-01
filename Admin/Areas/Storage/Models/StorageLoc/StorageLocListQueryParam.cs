using shunda.Core;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocListQueryParam : ListQueryParam
{
    public int ShelfId { get; set; }

    public int? RowNumQuery { get; set; }

    public int? ColNumQuery { get; set; }
}