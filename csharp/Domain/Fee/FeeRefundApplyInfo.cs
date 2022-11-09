using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

/// <summary>
/// 订单退赔款申请
/// </summary>
public class FeeRefundApplyInfo : Entity
{
    /// <summary>
    /// 物流订单
    /// </summary>
    [Required(ErrorMessage = "物流订单不能为空！")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 会员
    /// </summary>
    [Required(ErrorMessage = "会员不能为空！")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(FeeRefundTypeModelBinder))]
    public virtual FeeRefundType Type { get; set; } = FeeRefundType.None;

    [Required(ErrorMessage = "退赔原因不能为空！")]
    public virtual FeeRefundReasonInfo Reason { get; set; } = new FeeRefundReasonInfo();

    /// <summary>
    /// 退赔款原因
    /// </summary>
    [Required(ErrorMessage = "退赔款原因不能为空！")]
    [MaxLength(300, ErrorMessage = "退赔款原因长度不能大于300！")]
    public virtual string ReasonContent { get; set; } = string.Empty;

    /// <summary>
    /// 退赔金额
    /// </summary>
    [Required(ErrorMessage = "退赔金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 退赔支付方式
    /// </summary>
    [Required(ErrorMessage = "退赔支付方式不能为空！")]
    [ModelBinder(typeof(FeeRefundPayTypeModelBinder))]
    public virtual FeeRefundPayType PayType { get; set; } = FeeRefundPayType.None;

    /// <summary>
    /// 状态
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(FeeRefundApplyStatusModelBinder))]
    public virtual FeeRefundApplyStatus Status { get; set; } = FeeRefundApplyStatus.None;

    /// <summary>
    /// 申请人
    /// </summary>
    [Required(ErrorMessage = "申请人不能为空！")]
    public virtual UserInfo ApplyUser { get; set; } = new UserInfo();

    /// <summary>
    /// 申请时间
    /// </summary>
    public virtual DateTime? ApplyTime { get; set; }

    /// <summary>
    /// 申请备注
    /// </summary>
    [MaxLength(300, ErrorMessage = "申请备注长度不能大于300！")]
    public virtual string ApplyMemo { get; set; } = string.Empty;

    /// <summary>
    /// 审核人
    /// </summary>
    public virtual UserInfo? ApproveUser { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public virtual DateTime? ApproveTime { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [MaxLength(300, ErrorMessage = "审核备注长度不能大于300！")]
    public virtual string ApproveMemo { get; set; } = string.Empty;
}