using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressExpect;

public class ExpressExpectIndexModel
{
    [BindNever]
    public IList<ExpressExpectStatus> StatusList { get; set; } = new List<ExpressExpectStatus>();
}