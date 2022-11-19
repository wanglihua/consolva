using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressClaim;

public class ExpressClaimListModel : ListModel
{
    public ExpressClaimListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ExpressPhotoInfo> ExpressClaimList { get; set; } = new List<ExpressPhotoInfo>();
}