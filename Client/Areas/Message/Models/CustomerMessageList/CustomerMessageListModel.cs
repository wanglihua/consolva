using Newtonsoft.Json;

namespace shunda.Client.Areas.Message.Models.CustomerMessageList;

public class CustomerMessageListModel
{
    [JsonProperty("lastId")]
    public int LastId { get; set; }

    [JsonProperty("messageList")]
    public IList<CustomerMessageListItem> MessageList = new List<CustomerMessageListItem>();

    [JsonProperty("messageCount")]
    public int MessageCount { get; set; }
}