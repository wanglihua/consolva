using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressRemarkPresetSelect;

public class ExpressRemarkPresetSelectIndexModel
{
    [BindNever]
    public IList<ExpressRemarkType> TypeList { get; set; } = new List<ExpressRemarkType>();
}