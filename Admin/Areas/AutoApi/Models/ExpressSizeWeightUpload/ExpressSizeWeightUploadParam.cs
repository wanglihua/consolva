using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.ExpressSizeWeightUpload;

public class ExpressSizeWeightUploadParam {
    [JsonProperty("app_key")]
    public string AppKey { get; set; } = string.Empty;

    [JsonProperty("method")]
    public string Method { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    public string TimeStamp { get; set; } = string.Empty;

    [JsonProperty("sign")]
    public string Sign { get; set; } = string.Empty;

    [JsonProperty("data")]
    public ExpressSizeWeightData Data { get; set; } = new();

    public class ExpressSizeWeightData {
        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; } = string.Empty;

        [JsonProperty("width")]
        public string Width { get; set; } = string.Empty;

        [JsonProperty("length")]
        public string Length { get; set; } = string.Empty;

        [JsonProperty("height")]
        public string Height { get; set; } = string.Empty;

        [JsonProperty("weight")]
        public string Weight { get; set; } = string.Empty;
    }
}