using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

/// <summary>
/// 超级VIP折扣
/// </summary>
public class CustomerSuperVipDiscountInfo : Entity
{
    /// <summary>
    /// 会员
    /// </summary>
    [BindNever]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 折扣
    /// </summary>
    [Required(ErrorMessage = "折扣不能为空！")]
    public virtual decimal Discount { get; set; }

    /// <summary>
    /// 操作用户
    /// </summary>
    [BindNever]
    public virtual UserInfo User { get; set; } = new UserInfo();

    /// <summary>
    /// 描述
    /// </summary>
    public virtual string? Description { get; set; }

    #region cache

    /// <summary>
    /// 会员
    /// </summary>
    [Required(ErrorMessage = "会员不能为空！")]
    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}