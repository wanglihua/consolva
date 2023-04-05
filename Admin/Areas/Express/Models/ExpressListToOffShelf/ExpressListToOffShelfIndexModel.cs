using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Express.Models.ExpressListToOffShelf;

public class ExpressListToOffShelfIndexModel {
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<UserInfo> CheckinUserList { get; set; } = new List<UserInfo>();
}
