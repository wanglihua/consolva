using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain;

/// <summary>
/// 汇率
/// </summary>
public class ExchangeRateInfo : Entity
{
    /// <summary>
    /// 原货币代码
    /// </summary>
    [DisplayName("原货币代码")]
    [Required(ErrorMessage = "原货币代码不能为空！")]
    [MaxLength(10, ErrorMessage = "原货币代码长度不能大于10！")]
    public virtual string FromCurrencyCode { get; set; } = String.Empty;

    /// <summary>
    /// 原货币名称
    /// </summary>
    [DisplayName("原货币名称")]
    [Required(ErrorMessage = "原货币名称不能为空！")]
    [MaxLength(10, ErrorMessage = "原货币名称长度不能大于10！")]
    public virtual string FromCurrencyName { get; set; } = String.Empty;

    /// <summary>
    /// 折算货币代码
    /// </summary>
    [DisplayName("折算货币代码")]
    [Required(ErrorMessage = "折算货币代码不能为空！")]
    [MaxLength(10, ErrorMessage = "折算货币代码长度不能大于10！")]
    public virtual string ToCurrencyCode { get; set; } = String.Empty;

    /// <summary>
    /// 折算货币名称
    /// </summary>
    [DisplayName("折算货币名称")]
    [Required(ErrorMessage = "折算货币名称不能为空！")]
    [MaxLength(10, ErrorMessage = "折算货币名称长度不能大于10！")]
    public virtual string ToCurrencyName { get; set; } = String.Empty;

    /// <summary>
    /// 汇率
    /// </summary>
    [DisplayName("汇率")]
    [Required(ErrorMessage = "汇率不能为空！")]
    public virtual decimal Rate { get; set; }

    /// <summary>
    /// 汇率修正
    /// </summary>
    [DisplayName("汇率修正")]
    [Required(ErrorMessage = "汇率修正不能为空！")]
    public virtual decimal RateFix { get; set; }

    /// <summary>
    /// 更新标记，用来标记数据已更新，让 NHibernate 更新实体的更新时间，每次 Update 的时候不都更新这个时间就行
    /// </summary>
    public virtual int RefreshMark { get; set; }

    /// <summary>
    /// 获取最终汇率
    /// </summary>
    public virtual decimal GetFinalRate()
    {
        return Rate + RateFix;
    }
}