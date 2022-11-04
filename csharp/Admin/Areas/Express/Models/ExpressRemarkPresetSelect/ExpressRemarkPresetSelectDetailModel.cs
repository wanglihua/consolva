using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressRemarkPresetSelect;

public class ExpressRemarkPresetSelectDetailModel : ExpressRemarkPresetSelectSaveModel
{
    [BindNever]
    public IList<ExpressRemarkType> TypeList { get; set; } = new List<ExpressRemarkType>();
}