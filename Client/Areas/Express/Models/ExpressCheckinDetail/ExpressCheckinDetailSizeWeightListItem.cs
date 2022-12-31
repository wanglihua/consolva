using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinDetail;

public class ExpressCheckinDetailSizeWeightListItem {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("length")]
    public string Length { get; set; } = string.Empty;

    [JsonProperty("width")]
    public string Width { get; set; } = string.Empty;

    [JsonProperty("height")]
    public string Height { get; set; } = string.Empty;

    [JsonProperty("volumeWeight")]
    public string VolumeWeight { get; set; } = string.Empty;

    [JsonProperty("acutalWeight")]
    public string AcutalWeight { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}