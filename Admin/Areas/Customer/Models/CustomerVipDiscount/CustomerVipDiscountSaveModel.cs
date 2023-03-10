using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;

public class CustomerVipDiscountSaveModel {
    public int CustomerVipDiscountId { get; set; }

    /// <summary>
    /// 客户VIP等级
    /// </summary>
    [Required(ErrorMessage = "客户VIP等级不能为空！")]
    [ModelBinder(typeof(CustomerVipRankModelBinder))]
    public CustomerVipRank Rank { get; set; } = CustomerVipRank.None;

    /// <summary>
    /// 历史总积分
    /// </summary>
    [Required(ErrorMessage = "历史总积分不能为空！")]
    [Range(0, int.MaxValue, ErrorMessage = "历史总积分必须大于零！")]
    public decimal Score { get; set; }

    /// <summary>
    /// 折扣
    /// </summary>
    [Required(ErrorMessage = "折扣不能为空！")]
    [Range(0, 100, ErrorMessage = "折扣只能在0和100之间！")]
    public decimal Discount { get; set; }

    /// <summary>
    /// 是否可以使用优惠券
    /// </summary>
    public bool CanCoupon { get; set; }
}