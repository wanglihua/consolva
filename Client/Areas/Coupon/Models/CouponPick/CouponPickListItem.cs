using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponPick;

public class CouponPickListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("rule")]
    public string Rule { get; set; } = string.Empty;

    [JsonProperty("ruleDesc")]
    public string RuleDesc { get; set; } = string.Empty;
}