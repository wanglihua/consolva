using shunda.Core;

namespace shunda.Admin.Models.UserLoginLog;

public class UserLoginLogListQueryParam : ListQueryParam
{
    public string? UserQuery { get; set; }
}