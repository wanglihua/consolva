using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Storage.Models.StorageLoc;

public class StorageLocPreviewParam
{
    [Required(ErrorMessage = "货架Id不能为空！")]
    public int ShelfId { get; set; }
}