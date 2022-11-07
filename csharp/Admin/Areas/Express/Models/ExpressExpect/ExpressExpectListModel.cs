using shunda.Core;
using shunda.Domain.Express;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListModel : ListModel
{
    public ExpressExpectListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public string ExpressExpectAlias { get; set; } = string.Empty;

    [BindNever]
    public string CustomerAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<ExpressExpectInfo> ExpressExpectList { get; set; } = new List<ExpressExpectInfo>();
}