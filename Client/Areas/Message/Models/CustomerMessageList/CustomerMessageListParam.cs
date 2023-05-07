using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageList;

public class CustomerMessageListParam
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }
}