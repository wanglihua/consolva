using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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
    [Required(ErrorMessage = "原货币代码不能为空！")]
    [ModelBinder(typeof(CurrencyTypeModelBinder))]
    public virtual CurrencyType FromCurrency { get; set; } = CurrencyType.None;

    /// <summary>
    /// 折算货币代码
    /// </summary>
    [Required(ErrorMessage = "折算货币代码不能为空！")]
    [MaxLength(10, ErrorMessage = "折算货币代码长度不能大于10！")]
    [ModelBinder(typeof(CurrencyTypeModelBinder))]
    public virtual CurrencyType ToCurrency { get; set; } = CurrencyType.None;

    /// <summary>
    /// 汇率
    /// </summary>
    [Required(ErrorMessage = "汇率不能为空！")]
    public virtual decimal Rate { get; set; }

    /// <summary>
    /// 汇率修正
    /// </summary>
    [Required(ErrorMessage = "汇率修正不能为空！")]
    public virtual decimal RateFix { get; set; }

    /// <summary>
    /// 更新标记，用来标记数据已更新，让 NHibernate 更新实体的更新时间，每次 Update 的时候都更新这个时间就行
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