using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 17track运输商
/// </summary>
public class SeventeenTrackCarrierInfo : Entity {
    /// <summary>
    /// 编号，非空
    /// </summary>
    public virtual int CarrierKey { get; set; } = 0;

    /// <summary>
    /// 国家编号，非空
    /// </summary>
    public virtual int CountryCode { get; set; } = 0;

    /// <summary>
    /// 国家代码，非空
    /// </summary>
    public virtual string CountryIso { get; set; } = string.Empty;

    /// <summary>
    /// email，非空
    /// </summary>
    public virtual string Email { get; set; } = string.Empty;

    /// <summary>
    /// 电话，非空
    /// </summary>
    public virtual string Tel { get; set; } = string.Empty;

    /// <summary>
    /// 网址，非空
    /// </summary>
    public virtual string Url { get; set; } = string.Empty;

    /// <summary>
    /// 名称，非空
    /// </summary>
    public virtual string CarrierName { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称1，非空
    /// </summary>
    public virtual string NameZhCn { get; set; } = string.Empty;

    /// <summary>
    /// 中文名称2，非空
    /// </summary>
    public virtual string NameZhHk { get; set; } = string.Empty;

    /// <summary>
    /// 分组编号，非空
    /// </summary>
    public virtual string CarrierGroup { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<SeventeenTrackCarrierInfoValidator, SeventeenTrackCarrierInfo>(this);
    }
}