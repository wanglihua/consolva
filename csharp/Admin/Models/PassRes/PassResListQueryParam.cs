using shunda.Core;

namespace shunda.Admin.Models.PassRes;

public class PassResListQueryParam : ListQueryParam
{
    public int CateIdQuery { get; set; }

    public string? NameQuery { get; set; }
}