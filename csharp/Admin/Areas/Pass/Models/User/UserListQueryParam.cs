using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.User;

public class UserListQueryParam : ListQueryParam
{
    public string? LoginNameQuery { get; set; }

    public string? UserNameQuery { get; set; }
}