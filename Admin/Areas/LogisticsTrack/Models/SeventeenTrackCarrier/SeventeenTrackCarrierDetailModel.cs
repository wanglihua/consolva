namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierDetailModel {
    /// <summary>
    /// Id
    /// </summary>
    public int SeventeenTrackCarrierId { get; set; }

    /// <summary>
    /// 编号，非空
    /// </summary>
    public int CarrierKey { get; set; } = 0;

    /// <summary>
    /// 国家编号，非空
    /// </summary>
    public int CountryCode { get; set; } = 0;

    /// <summary>
    /// 国家代码，非空
    /// </summary>
    public string CountryIso { get; set; } = string.Empty;

    /// <summary>
    /// email，非空
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 电话，非空
    /// </summary>
    public string Tel { get; set; } = string.Empty;

    /// <summary>
    /// 网址，非空
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 名称，非空
    /// </summary>
    public string CarrierName { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称1，非空
    /// </summary>
    public string NameZhCn { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称2，非空
    /// </summary>
    public string NameZhHk { get; set; } = string.Empty;

    /// <summary>
    /// 分组编号，非空
    /// </summary>
    public string CarrierGroup { get; set; } = string.Empty;
}