using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.OrderFee.Models.CustomerCharge;

public class CustomerChargeDetailModel
{
    [Required(ErrorMessage = "会员ID不能为空！")]
    public string CustomerCode { get; set; }

    [Required(ErrorMessage = "充值金额不能为空！")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "手工充值原因不能为空！")]
    public string ChargeReason { get; set; }

    // todo 马上补上
    // [BindNever]
    // public IList<ManualChargeReasonInfo> ChargeReasonList { get; set; }
}