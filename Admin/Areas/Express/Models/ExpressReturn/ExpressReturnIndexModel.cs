using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Express.Models.ExpressReturn;

public class ExpressReturnIndexModel {
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<ExpressStatus> ReturnStatusList { get; set; } = ExpressStatus.GetReturnList();
}