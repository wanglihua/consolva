using shunda.Core;

namespace shunda.Admin.Models.Warehouse;

public class WarehouseListQueryParam : ListQueryParam
{
    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}