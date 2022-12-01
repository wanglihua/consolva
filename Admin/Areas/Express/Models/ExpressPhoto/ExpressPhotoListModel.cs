using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressPhoto;

public class ExpressPhotoListModel : ListModel
{
    public ExpressPhotoListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ExpressPhotoInfo> ExpressPhotoList { get; set; } = new List<ExpressPhotoInfo>();
}