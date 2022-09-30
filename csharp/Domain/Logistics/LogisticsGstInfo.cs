using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// GST
/// </summary>
public class LogisticsGstInfo : Entity
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 计算基数
    /// </summary>
    [Required(ErrorMessage = "计算基数不能为空！")]
    [ModelBinder(typeof(FeeCalcBaseTypeModelBinder))]
    public virtual FeeCalcBaseType CalcBaseType { get; set; } = FeeCalcBaseType.None;

    /// <summary>
    /// 启征金额
    /// </summary>
    [Required(ErrorMessage = "启征金额不能为空！")]
    public virtual decimal StartAmount { get; set; }

    /// <summary>
    /// 费率
    /// </summary>
    [Required(ErrorMessage = "费率不能为空！")]
    public virtual decimal FeeRate { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public virtual int? OrderNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(LogisticsGstStatusModelBinder))]
    public virtual LogisticsGstStatus Status { get; set; } = LogisticsGstStatus.None;
}