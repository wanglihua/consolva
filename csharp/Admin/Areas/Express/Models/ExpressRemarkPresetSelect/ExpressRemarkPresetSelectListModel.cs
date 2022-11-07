using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressRemarkPresetSelect;

public class ExpressRemarkPresetSelectListModel : ListModel
{
    public string ExpressRemarkPresetSelectAlias = string.Empty;

    public ExpressRemarkPresetSelectListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ExpressRemarkPresetSelectInfo> ExpressRemarkPresetSelectList { get; set; } = new List<ExpressRemarkPresetSelectInfo>();
}