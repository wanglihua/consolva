using Newtonsoft.Json;

namespace shunda.Admin.Areas.App.Models.ExpressPhoto;

public class ExpressPhotoGetOrderCodeByPackBoxCodeModel {
    [JsonProperty("packBoxCode")]
    public string PackBoxCode { get; set; } = string.Empty;

    [JsonProperty("orderCode")]
    public string OrderCode { get; set; } = string.Empty;
}