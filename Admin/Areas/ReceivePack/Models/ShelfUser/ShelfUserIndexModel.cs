using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ShelfUser;

public class ShelfUserIndexModel
{
    [BindNever]
    public IList<ShelfUserType> TypeList { get; set; } = ShelfUserType.GetList();
}