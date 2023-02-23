using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Setting.Models.AutoMeasureMachine;

public class AutoMeasureMachineListModel : ListModel {
    public AutoMeasureMachineListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<AutoMeasureMachineInfo> AutoMeasureMachineList { get; set; } = new List<AutoMeasureMachineInfo>();
}