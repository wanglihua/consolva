using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.ExpressOffShelf;

public class ExpressOffShelfShelfListModel {
    [JsonProperty("shelfList")]
    public IList<ExpressOffShelfShelfListItem> ShelfList { get; set; } = new List<ExpressOffShelfShelfListItem>();
}