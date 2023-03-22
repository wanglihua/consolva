using Newtonsoft.Json;

namespace shunda.Client.Areas.Coupon.Models.CouponMine;

public class CouponMineListItem
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("rule")]
    public string Rule { get; set; } = string.Empty;

    [JsonProperty("ruleDesc")]
    public string RuleDesc { get; set; } = string.Empty;

    [JsonProperty("expire")]
    public string Expire { get; set; } = string.Empty;

    [JsonProperty("follow")]
    public bool Follow { get; set; }

    [JsonProperty("batchName")]
    public string BatchName { get; set; } = string.Empty;
}