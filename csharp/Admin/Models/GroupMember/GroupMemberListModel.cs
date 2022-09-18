using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.GroupMember;

public class GroupMemberListModel : ListModel
{
    public GroupMemberListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<GroupMemberInfo> GroupMemberList { get; set; } = new List<GroupMemberInfo>();
}