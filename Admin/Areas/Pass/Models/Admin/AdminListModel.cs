using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.Admin;

public class AdminListModel : ListModel
{
    public AdminListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<AdminInfo> AdminList { get; set; } = new List<AdminInfo>();
}