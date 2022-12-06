using Newtonsoft.Json;

namespace shunda.Core.Web;

public class JsonResultData
{
    [JsonProperty("success")]
    public bool Success { get; set; } = true;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public object? Data { get; set; } = null;
}