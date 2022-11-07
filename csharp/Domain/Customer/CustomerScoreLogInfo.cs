using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.Order;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户积分日志
/// </summary>
public class CustomerScoreLogInfo : Entity
{
    /// <summary>
    /// 客户id
    /// </summary>
    [DisplayName("客户id")]
    [Required(ErrorMessage = "客户id不能为空！")]
    public virtual CustomerInfo Customer { get; set; } = new CustomerInfo();

    /// <summary>
    /// 积分变动类型
    /// </summary>
    [DisplayName("积分变动类型")]
    [Required(ErrorMessage = "积分变动类型不能为空！")]
    [ModelBinder(typeof(CustomerScoreTypeModelBinder))]
    public virtual CustomerScoreType ScoreType { get; set; } = CustomerScoreType.None;

    /// <summary>
    /// 变动积分
    /// </summary>
    [DisplayName("变动积分")]
    [Required(ErrorMessage = "变动积分不能为空！")]
    public virtual decimal Score { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [Required(ErrorMessage = "金额不能为空！")]
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 是否有效积分
    /// </summary>
    [Required(ErrorMessage = "是否有效积分不能为空！")]
    public virtual bool IsValidScore { get; set; }

    /// <summary>
    /// 变动描述
    /// </summary>
    [DisplayName("变动描述")]
    [Required(ErrorMessage = "变动描述不能为空！")]
    [MaxLength(300, ErrorMessage = "变动描述长度不能大于300！")]
    public virtual string Description { get; set; } = string.Empty; // 非空字段

    /// <summary>
    /// 订单支付
    /// </summary>
    public virtual LogisticsOrderPayInfo? LogisticsOrderPay { get; set; }

    /// <summary>
    /// 订单
    /// </summary>
    public virtual LogisticsOrderInfo? LogisticsOrder { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public virtual string? LogisticsOrderCode { get; set; }

    /// <summary>
    /// 订单会员
    /// </summary>
    public virtual CustomerInfo? LogisticsOrderCustomer { get; set; }
}