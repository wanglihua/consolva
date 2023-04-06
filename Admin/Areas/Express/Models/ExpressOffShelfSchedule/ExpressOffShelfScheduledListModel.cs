using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressOffShelfSchedule;

public class ExpressOffShelfScheduledListModel : ListModel {
    public ExpressOffShelfScheduledListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<ExpressShelfScheduleInfo> ScheduledList { get; set; } = new List<ExpressShelfScheduleInfo>();
}