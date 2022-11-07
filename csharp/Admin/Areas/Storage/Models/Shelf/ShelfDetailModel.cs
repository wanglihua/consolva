using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Storage.Models.Shelf;

public class ShelfDetailModel
{
    /// <summary>
    ///     Shelf Id，显示详情界面时，用于 HTTP 接收参数
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Warehouse Id，显示详情界面时，用于 HTTP 接收参数
    /// </summary>
    public int WarehouseId { get; set; }

    public ShelfInfo Shelf { get; set; } = new();

    [BindNever]
    public IList<ShelfStatus> ShelfStatusList { get; set; } = new List<ShelfStatus>();

    [BindNever]
    public IList<ShelfType> ShelfTypeList { get; set; } = new List<ShelfType>();

    [BindNever]
    public IList<ShelfCate> ShelfCateList { get; set; } = new List<ShelfCate>();
}