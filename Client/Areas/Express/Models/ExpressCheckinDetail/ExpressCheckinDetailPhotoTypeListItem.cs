using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinDetail;

public class ExpressCheckinDetailPhotoTypeListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("photoCount")]
    public int PhotoCount { get; set; }

    [JsonProperty("photoList")]
    public IList<ExpressCheckinDetailPhotoListItem> PhotoList = new List<ExpressCheckinDetailPhotoListItem>();
}