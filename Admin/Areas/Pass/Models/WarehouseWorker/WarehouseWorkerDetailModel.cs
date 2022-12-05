using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerDetailModel : WarehouseWorkerSaveModel
{
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<WarehouseWorkerType> TypeList { get; set; } = WarehouseWorkerType.GetList();

    [BindNever]
    public WarehouseWorkerInfo WarehouseWorker { get; set; } = new ();

    [BindNever]
    public string UserQuery { get; set; } = string.Empty;

    [BindNever]
    public IList<GroupInfo> GroupList { get; set; } = new List<GroupInfo>();
}