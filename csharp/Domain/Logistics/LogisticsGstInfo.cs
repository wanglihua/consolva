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
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 计算基数
    /// </summary>
    [DisplayName("计算基数")]
    [Required(ErrorMessage = "计算基数不能为空！")]
    [ModelBinder(typeof(FeeCalcBaseTypeModelBinder))]
    public virtual FeeCalcBaseType CalcBaseType { get; set; } = FeeCalcBaseType.None;

    /// <summary>
    /// 启征金额
    /// </summary>
    [DisplayName("启征金额")]
    [Required(ErrorMessage = "启征金额不能为空！")]
    public virtual decimal StartAmount { get; set; }

    /// <summary>
    /// 费率
    /// </summary>
    [DisplayName("费率")]
    [Required(ErrorMessage = "费率不能为空！")]
    public virtual decimal FeeRate { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [DisplayName("排序")]
    public virtual int? OrderNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(LogisticsGstStatusModelBinder))]
    public virtual LogisticsGstStatus Status { get; set; } = LogisticsGstStatus.None;
}