using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.Group;

public class GroupListQueryParam : ListQueryParam
{
    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }

    public string? UserQuery { get; set; }
}