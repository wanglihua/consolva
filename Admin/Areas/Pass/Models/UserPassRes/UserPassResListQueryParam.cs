using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.UserPassRes;

public class UserPassResListQueryParam : ListQueryParam
{
    public int UserId { get; set; }

    public string? CateNameQuery { get; set; }

    public string? PassResNameQuery { get; set; }
}