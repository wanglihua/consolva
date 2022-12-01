using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ShelfUser;

public class ShelfUserListModel : ListModel
{
    public ShelfUserListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ShelfUserInfo> ShelfUserList { get; set; } = new List<ShelfUserInfo>();
}