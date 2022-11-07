using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.GroupMember;

public class GroupMemberIndexModel
{
    public int GroupId { get; set; }

    public GroupInfo Group { get; set; } = new();
}