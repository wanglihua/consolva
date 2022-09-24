using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfListQueryParam : ListQueryParam
{
    public int WarehouseId { get; set; }

    [ModelBinder(typeof(ShelfTypeModelBinder))]
    public ShelfType? ShelfTypeQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}