using shunda.Core;
using shunda.Domain.Express;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Express.Models.ExpressRemarkPresetSelect;

public class ExpressRemarkPresetSelectListModel : ListModel
{
    public ExpressRemarkPresetSelectListModel(ListPager listPager) : base(listPager)
    {
    }

    public string ExpressRemarkPresetSelectAlias = string.Empty;    

    [BindNever]
    public IList<ExpressRemarkPresetSelectInfo> ExpressRemarkPresetSelectList { get; set; } = new List<ExpressRemarkPresetSelectInfo>();
}