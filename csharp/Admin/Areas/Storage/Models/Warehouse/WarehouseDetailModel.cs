using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Warehouse;

public class WarehouseDetailModel
{
    public WarehouseWithAddressDescriptionInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<WarehouseStatus> WarehouseStatusList { get; set; } = new List<WarehouseStatus>();
}