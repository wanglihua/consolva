using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Admin.Models.Warehouse;

public class WarehouseListModel : ListModel
{
    public WarehouseListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();
}