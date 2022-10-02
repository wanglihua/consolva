using Newtonsoft.Json;

namespace shunda.Core;

public class CheckResult
{
    public static CheckResult OK(string message) => new CheckResult(true, message);

    public static CheckResult FAIL(string message) => new CheckResult(false, message);

    public CheckResult(bool success, string message)
    {
        this.Success = success;
        this.Message = message;
    }

    [JsonProperty("success")]
    public bool Success { get; private set; }

    [JsonProperty("message")]
    public string Message { get; private set; }
}