using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 会员积分规则
/// 使用这些规则时要检查 金额 或 积分 是不是0
/// </summary>
public class CustomerScoreRuleInfo : Entity
{
    /// <summary>
    /// 类型
    /// </summary>
    [DisplayName("类型")]
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(CustomerScoreTypeModelBinder))]
    public virtual CustomerScoreType Type { get; set; } = CustomerScoreType.None;

    /// <summary>
    /// 金额
    /// </summary>
    [DisplayName("金额")]
    [Required(ErrorMessage = "金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 积分
    /// </summary>
    [DisplayName("积分")]
    [Required(ErrorMessage = "积分不能为空！")]
    public virtual decimal Score { get; set; }
}