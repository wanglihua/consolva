using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerDetailParam
{
    public int? Id { get; set; }

    public int? WarehouseId { get; set; }

    [ModelBinder(typeof(WarehouseWorkerTypeModelBinder))]
    public WarehouseWorkerType? Type { get; set; }

    public string? UserQuery { get; set; }
}
