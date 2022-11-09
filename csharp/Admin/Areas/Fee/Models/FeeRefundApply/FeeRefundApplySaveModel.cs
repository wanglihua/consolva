using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApply;

public class FeeRefundApplySaveModel
{
    public int ApplyId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    public string OrderCode { get; set; }

    /// <summary>
    /// 客户编号
    /// </summary>
    public string CustomerCode { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 退赔原因
    /// </summary>
    [Required(ErrorMessage = "退赔原因不能为空！")]
    public int ReasonId { get; set; }

    /// <summary>
    /// 申请备注
    /// </summary>
    [MaxLength(255, ErrorMessage = "申请备注长度不能大于255！")]
    public string ApplyMemo { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(FeeRefundTypeModelBinder))]
    public FeeRefundType Type { get; set; }
}