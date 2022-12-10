using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfDetailModel : ShelfSaveModel
{
    [BindNever]
    public IList<ShelfStatus> ShelfStatusList { get; set; } = new List<ShelfStatus>();

    [BindNever]
    public IList<ShelfType> ShelfTypeList { get; set; } = new List<ShelfType>();

    [BindNever]
    public IList<ShelfCate> ShelfCateList { get; set; } = new List<ShelfCate>();
}