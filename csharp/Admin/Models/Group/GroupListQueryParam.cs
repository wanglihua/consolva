using shunda.Core;

namespace shunda.Admin.Models.Group;

public class GroupListQueryParam : ListQueryParam
{
    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }

    public string? UserQuery { get; set; }
}