using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressOffShelfSchedule;

public class ExpressOffShelfSchedulingListModel : ListModel {
    public ExpressOffShelfSchedulingListModel(ListPager listPager) : base(listPager) {
    }

    // [BindNever]
    // public IList<ShelfInfo> ShelfList { get; set; }        

    [BindNever]
    public IList<ExpressInfo> SchedulingList { get; set; } = new List<ExpressInfo>();
}