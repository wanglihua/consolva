using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressVideo;

public class ExpressVideoListModel : ListModel
{
    public ExpressVideoListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ExpressVideoInfo> ExpressVideoList { get; set; } = new List<ExpressVideoInfo>();
}