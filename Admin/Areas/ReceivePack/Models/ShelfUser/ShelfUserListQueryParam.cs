using shunda.Core;

namespace shunda.Admin.Areas.ReceivePack.Models.ShelfUser;

public class ShelfUserListQueryParam : ListQueryParam
{
    public string? ShelfNameQuery { get; set; }

    public string? UserQuery { get; set; }
}