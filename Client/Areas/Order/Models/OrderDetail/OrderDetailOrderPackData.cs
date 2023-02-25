using Newtonsoft.Json;

namespace shunda.Client.Areas.Order.Models.OrderDetail;

public class OrderDetailOrderPackData {
    [JsonProperty("totalActualWeight")]
    public string TotalActualWeight { get; set; } = string.Empty;

    [JsonProperty("totalVolumeWeight")]
    public string TotalVolumeWeight { get; set; } = string.Empty;

    [JsonProperty("totalVolume")]
    public string TotalVolume { get; set; } = string.Empty;

    [JsonProperty("totalPackageCount")]
    public int TotalPackageCount { get; set; }

    [JsonProperty("maxSideLength")]
    public string MaxSideLength { get; set; } = string.Empty;

    [JsonProperty("maxVolumeSinglePack")]
    public string MaxVolumeSinglePack { get; set; } = string.Empty;

    [JsonProperty("maxWeightSinglePack")]
    public string MaxWeightSinglePack { get; set; } = string.Empty;
}