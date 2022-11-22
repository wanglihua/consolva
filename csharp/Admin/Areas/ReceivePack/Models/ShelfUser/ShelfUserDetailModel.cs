using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.ShelfUser;

public class ShelfUserDetailModel : ShelfUserSaveModel
{
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<ShelfInfo> ShelfList { get; set; } = new List<ShelfInfo>();

    [BindNever]
    public IList<ShelfUserType> TypeList { get; set; } = new List<ShelfUserType>();
}