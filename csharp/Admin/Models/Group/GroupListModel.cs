using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.Group;

public class GroupListModel : ListModel
{
    public GroupListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<GroupInfo> GroupList { get; set; } = new List<GroupInfo>();
}