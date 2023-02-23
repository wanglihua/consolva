using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.ExpressSizeWeightUpload;

public class ExpressSizeWeightUploadResult {
    [JsonProperty("result")]
    public bool Result { get; set; }
}