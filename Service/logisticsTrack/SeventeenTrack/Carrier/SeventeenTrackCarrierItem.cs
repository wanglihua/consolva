using Newtonsoft.Json;

namespace shunda.Service.logisticsTrack.SeventeenTrack.Carrier;

public class SeventeenTrackCarrierItem {
    /// <summary>
    /// 编号，非空
    /// </summary>
    [JsonProperty("key")]
    public int CarrierKey { get; set; } = 0;

    /// <summary>
    /// 国家编号，非空
    /// </summary>
    [JsonProperty("_country")]
    public int CountryCode { get; set; } = 0;

    /// <summary>
    /// 国家代码，非空
    /// </summary>
    [JsonProperty("_country_iso")]
    public string CountryIso { get; set; } = string.Empty;

    /// <summary>
    /// email，非空
    /// </summary>
    [JsonProperty("_email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 电话，非空
    /// </summary>
    [JsonProperty("_tel")]
    public string Tel { get; set; } = string.Empty;

    /// <summary>
    /// 网址，非空
    /// </summary>
    [JsonProperty("_url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 名称，非空
    /// </summary>
    [JsonProperty("_name")]
    public string CarrierName { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称1，非空
    /// </summary>
    [JsonProperty("_name_zh-cn")]
    public string NameZhCn { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称2，非空
    /// </summary>
    [JsonProperty("_name_zh-hk")]
    public string NameZhHk { get; set; } = string.Empty;

    /// <summary>
    /// 分组编号，非空
    /// </summary>
    [JsonProperty("_group")]
    public string CarrierGroup { get; set; } = string.Empty;
}