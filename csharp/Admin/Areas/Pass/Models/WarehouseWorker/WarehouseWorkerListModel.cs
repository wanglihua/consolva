using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerListModel : ListModel
{
    public WarehouseWorkerListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<WarehouseWorkerInfo> WarehouseWorkerList { get; set; } = new List<WarehouseWorkerInfo>();

    [BindNever]
    public string WarehouseAlias { get; set; } = string.Empty;

    [BindNever]
    public string UserAlias { get; set; } = string.Empty;
}