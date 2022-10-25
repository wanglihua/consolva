using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

/// <summary>
/// VIP折扣设置
/// </summary>
public class CustomerVipDiscountInfo : Entity
{
    /// <summary>
    /// 会员VIP等级
    /// </summary>
    [Required(ErrorMessage = "会员VIP等级不能为空！")]
    [ModelBinder(typeof(CustomerVipRankModelBinder))]
    public virtual CustomerVipRank Rank { get; set; } = CustomerVipRank.None;

    /// <summary>
    /// 历史总积分
    /// </summary>
    [Required(ErrorMessage = "历史总积分不能为空！")]
    public virtual decimal Score { get; set; }

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
}
