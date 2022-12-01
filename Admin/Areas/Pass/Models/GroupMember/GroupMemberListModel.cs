using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.GroupMember;

public class GroupMemberListModel : ListModel
{
    public GroupMemberListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<GroupMemberInfo> GroupMemberList { get; set; } = new List<GroupMemberInfo>();
}