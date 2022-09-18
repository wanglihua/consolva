using shunda.Domain.Pass;

namespace shunda.Admin.Models.GroupMember;

public class GroupMemberIndexModel
{
    public int GroupId { get; set; }

    public GroupInfo Group { get; set; } = new GroupInfo();
}