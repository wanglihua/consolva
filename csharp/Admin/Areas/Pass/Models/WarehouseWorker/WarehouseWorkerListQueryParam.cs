using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerListQueryParam : ListQueryParam
{
    public int WarehouseIdQuery { get; set; }

    [ModelBinder(typeof(WarehouseWorkerTypeModelBinder))]
    public WarehouseWorkerType? TypeQuery { get; set; }

    public string? UserQuery { get; set; }
}