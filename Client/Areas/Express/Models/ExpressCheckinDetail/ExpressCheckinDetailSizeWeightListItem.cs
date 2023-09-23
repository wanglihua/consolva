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

    /// <summary>
    /// 单箱体积重
    /// </summary>
    [JsonProperty("volumeWeight")]
    public string VolumeWeight { get; set; } = string.Empty;

    /// <summary>
    /// 单箱实际重
    /// </summary>
    [JsonProperty("actualWeight")]
    public string ActualWeight { get; set; } = string.Empty;

    [JsonProperty("packageCount")]
    public int PackageCount { get; set; }
}