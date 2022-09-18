using shunda.Core;

namespace shunda.Admin.Models.GroupMember;

public class GroupMemberListQueryParam : ListQueryParam
{
    public int GroupId { get; set; }

    public string? LoginNameQuery { get; set; }

    public string? UserNameQuery { get; set; }
}