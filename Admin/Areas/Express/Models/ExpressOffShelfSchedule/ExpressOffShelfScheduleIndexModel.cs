using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Express.Models.ExpressOffShelfSchedule;

public class ExpressOffShelfScheduleIndexModel {
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<UserInfo> OffShelfUserList { get; set; } = new List<UserInfo>();

    [BindNever]
    public IList<ShelfInfo> ShelfList { get; set; } = new List<ShelfInfo>();

    [BindNever]
    public IList<LogisticsModeInfo> FirstModeList { get; set; } = new List<LogisticsModeInfo>();
}
