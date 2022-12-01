using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.StorageLocSpec;

public class StorageLocSpecListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(ShelfTypeModelBinder))]
    public ShelfType? TypeQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? NameQuery { get; set; }
}