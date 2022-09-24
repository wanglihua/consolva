using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfListModel : ListModel
{
    public ShelfListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ShelfInfo> ShelfList { get; set; } = new List<ShelfInfo>();
}