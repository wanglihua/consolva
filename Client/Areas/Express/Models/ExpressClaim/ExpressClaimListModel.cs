using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimListModel
{
    [JsonProperty("expressClaimCount")]
    public int ExpressClaimCount { get; set; }

    [JsonProperty("expressClaimList")]
    public IList<ExpressClaimListItem> ExpressClaimList { get; set; } = new List<ExpressClaimListItem>();

    [JsonProperty("lastExpressId")]
    public int LastExpressId { get; set; }
}