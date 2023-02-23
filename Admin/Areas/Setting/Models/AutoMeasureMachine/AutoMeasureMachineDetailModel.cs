using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Setting.Models.AutoMeasureMachine;

public class AutoMeasureMachineDetailModel : AutoMeasureMachineSaveModel {
    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();
}