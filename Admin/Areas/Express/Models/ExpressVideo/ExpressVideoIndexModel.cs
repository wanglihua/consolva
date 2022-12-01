using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressVideo;

public class ExpressVideoIndexModel
{
    [BindNever]
    public IList<ExpressVideoType> VideoTypeList { get; set; } = ExpressVideoType.GetList();

    [BindNever]
    public bool CanDelete { get; set; }
}