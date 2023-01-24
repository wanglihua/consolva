using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressRemark;

public class ExpressRemarkListModel {
    [BindNever]
    public int ExpressId { get; set; }

    [BindNever]
    public IList<ExpressRemarkInfo> ExpressRemarkList { get; set; } = new List<ExpressRemarkInfo>();
}