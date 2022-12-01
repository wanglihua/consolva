using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Fee.Models.CustomerCharge;

public class CustomerChargeDetailModel
{
    [Required(ErrorMessage = "会员ID不能为空！")]
    public string CustomerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "充值金额不能为空！")]
    public decimal Amount { get; set; } = 0;

    [Required(ErrorMessage = "手工充值原因不能为空！")]
    public string ChargeReason { get; set; } = string.Empty;

    [BindNever]
    public IList<ManualChargeReasonInfo> ChargeReasonList { get; set; } = new List<ManualChargeReasonInfo>();
}