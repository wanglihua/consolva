using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerSaveModel
{
    [Required(ErrorMessage = "仓库不能为空！")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(WarehouseWorkerTypeModelBinder))]
    public WarehouseWorkerType WorkerType { get; set; } = WarehouseWorkerType.None;

    public IList<int> UserIdList { get; set; } = new List<int>();
}