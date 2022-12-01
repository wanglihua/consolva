using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerIndexModel
{
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<WarehouseWorkerType> TypeList { get; set; } = WarehouseWorkerType.GetList();
}