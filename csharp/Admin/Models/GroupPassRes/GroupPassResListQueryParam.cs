using shunda.Core;

namespace shunda.Admin.Models.GroupPassRes;

public class GroupPassResListQueryParam : ListQueryParam
{
    public int GroupId { get; set; }

    public string? CateNameQuery { get; set; }

    public string? PassResNameQuery { get; set; }
}