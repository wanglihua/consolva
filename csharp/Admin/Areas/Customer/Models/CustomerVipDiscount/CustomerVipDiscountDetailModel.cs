using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;

public class CustomerVipDiscountDetailModel
{
    public int CustomerVipDiscountId { get; set; } 
    
    /// <summary>
    /// 会员VIP等级
    /// </summary>
    [DisplayName("会员VIP等级")]
    [Required(ErrorMessage = "会员VIP等级不能为空！")]
    [ModelBinder(typeof(CustomerVipRankModelBinder))]
    public virtual CustomerVipRank Rank { get; set; } = CustomerVipRank.None;

    /// <summary>
    /// 历史总积分
    /// </summary>
    [DisplayName("历史总积分")]
    [Required(ErrorMessage = "历史总积分不能为空！")]
    public virtual decimal Score { get; set; }

    /// <summary>
    /// 集运头程折扣
    /// </summary>
    [DisplayName("集运头程折扣")]
    [Required(ErrorMessage = "集运头程折扣不能为空！")]
    public virtual decimal Discount { get; set; }    

    [BindNever]
    public IList<CustomerVipRank> CustomerVipRankList { get; set; } = CustomerVipRank.GetList();
}