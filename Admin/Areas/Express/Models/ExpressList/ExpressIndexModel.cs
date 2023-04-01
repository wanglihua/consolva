using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Express.Models.ExpressList;

public class ExpressIndexModel {
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<UserInfo> CheckinUserList { get; set; } = new List<UserInfo>();

    [BindNever]
    public bool CanExpressForceDelete { get; set; }

    [BindNever]
    public bool CanExpressThrowAway { get; set; }
}