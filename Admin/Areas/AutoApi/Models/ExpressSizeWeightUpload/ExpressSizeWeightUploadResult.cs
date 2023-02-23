using Newtonsoft.Json;

namespace shunda.Admin.Areas.AutoApi.Models.ExpressSizeWeightUpload;

public class ExpressSizeWeightUploadResult {
    /// <summary>
    /// success / failure
    /// </summary>
    [JsonProperty("flag")]
    public string Flag { get; set; } = string.Empty;

    /// <summary>
    /// 正常时 true, 异常时 false
    /// </summary>
    [JsonProperty("data")]
    public bool Data { get; set; }

    /// <summary>
    /// 正常时置空
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 正常时置空, S01 系统异常
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
}