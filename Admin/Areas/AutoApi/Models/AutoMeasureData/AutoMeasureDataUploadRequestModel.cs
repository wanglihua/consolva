using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.AutoMeasureData;

public class AutoMeasureDataUploadRequestModel {
    [JsonProperty("app_key")]
    public string AppKey { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    public string TimeStamp { get; set; } = string.Empty;

    [JsonProperty("sign")]
    public string Sign { get; set; } = string.Empty;

    [JsonProperty("data")]
    public ModelData Data { get; set; } = new();

    public class ModelData {
        [JsonProperty("boxType")]
        public string BoxType { get; set; } = string.Empty;

        [JsonProperty("trackingNumber")]
        public string TrackingNumber { get; set; } = string.Empty;

        [JsonProperty("width")]
        public string Width { get; set; } = string.Empty;

        [JsonProperty("length")]
        public string Length { get; set; } = string.Empty;

        [JsonProperty("height")]
        public string Height { get; set; } = string.Empty;

        [JsonProperty("volume")]
        public string Volume { get; set; } = string.Empty;

        [JsonProperty("weight")]
        public string Weight { get; set; } = string.Empty;
    }
}