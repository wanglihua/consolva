using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressSizeWeight;

public class ExpressSizeWeightListModel : ListModel
{
    public ExpressSizeWeightListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ExpressSizeWeightInfo> ExpressSizeWeightList { get; set; } = new List<ExpressSizeWeightInfo>();
}