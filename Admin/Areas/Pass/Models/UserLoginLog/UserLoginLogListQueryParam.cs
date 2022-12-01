using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.UserLoginLog;

public class UserLoginLogListQueryParam : ListQueryParam
{
    public string? UserQuery { get; set; }
}