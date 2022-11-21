using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.Express;

public class ExpressListModel : ListModel
{
    public ExpressListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ExpressInfo> ExpressClaimList { get; set; } = new List<ExpressInfo>();
}