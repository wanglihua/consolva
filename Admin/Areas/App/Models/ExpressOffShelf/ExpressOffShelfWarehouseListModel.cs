using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.ExpressOffShelf;

public class ExpressOffShelfWarehouseListModel {
    [JsonProperty("warehouseList")]
    public IList<ExpressOffShelfWarehouseListItem> WarehouseList { get; set; } = new List<ExpressOffShelfWarehouseListItem>();
}