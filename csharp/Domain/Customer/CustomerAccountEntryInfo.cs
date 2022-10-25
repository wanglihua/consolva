using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.DataAnnotations;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户账户变动明细
/// </summary>
public class CustomerAccountEntryInfo : Entity
{
    /// <summary>
    /// 余额变动类型
    /// </summary>
    [ModelBinder(typeof(CustomerAccountChangeTypeModelBinder))]
    public virtual CustomerAccountChangeType ChangeType { get; set; } = CustomerAccountChangeType.None;

    /// <summary>
    /// 变动金额
    /// </summary>
    [Required(ErrorMessage = "变动金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 变动前余额
    /// </summary>
    [Required(ErrorMessage = "变动前余额不能为空！")]
    public virtual decimal AccountBefore { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    [Required(ErrorMessage = "变动后余额不能为空！")]
    public virtual decimal AccountAfter { get; set; }

    /// <summary>
    /// 变动描述
    /// </summary>
    [Required(ErrorMessage = "变动描述不能为空！")]
    [MaxLength(300, ErrorMessage = "变动描述长度不能大于300！")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 客户
    /// </summary>
    [EntityRequired(ErrorMessage = "客户不能为空！")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();
}