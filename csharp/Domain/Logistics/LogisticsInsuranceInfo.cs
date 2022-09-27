using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 保险
/// </summary>
public class LogisticsInsuranceInfo : Entity
{
    /// <summary>
    /// 名称
    /// </summary>
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50, ErrorMessage = "名称长度不能大于50！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 保险条款
    /// </summary>
    [DisplayName("保险条款")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 费用类型
    /// </summary>
    [DisplayName("费用类型")]
    [Required(ErrorMessage = "费用类型不能为空！")]
    [ModelBinder(typeof(LogisticsInsuranceTypeModelBinder))]
    public virtual LogisticsInsuranceType Type { get; set; } = LogisticsInsuranceType.None;

    /// <summary>
    /// 计算基数
    /// </summary>
    [DisplayName("计算基数")]
    [Required(ErrorMessage = "计算基数不能为空！")]
    [ModelBinder(typeof(FeeCalcBaseTypeModelBinder))]
    public virtual FeeCalcBaseType CalcBaseType { get; set; } = FeeCalcBaseType.None;

    /// <summary>
    /// 费率/金额
    /// </summary>
    [DisplayName("费率/金额")]
    [Required(ErrorMessage = "费率/金额不能为空！")]
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
    [ModelBinder(typeof(LogisticsInsuranceStatusModelBinder))]
    public virtual LogisticsInsuranceStatus Status { get; set; } = LogisticsInsuranceStatus.None;
}