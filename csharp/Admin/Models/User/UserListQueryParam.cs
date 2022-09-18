using shunda.Core;

namespace shunda.Admin.Models.User;

public class UserListQueryParam : ListQueryParam
{
    public string? LoginNameQuery { get; set; }

    public string? UserNameQuery { get; set; }
}