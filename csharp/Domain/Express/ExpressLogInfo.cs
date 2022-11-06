using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单日志
/// </summary>
public class ExpressLogInfo : EntityNoUpdate
{
    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(ExpressLogTypeModelBinder))]
    public virtual ExpressLogType Type { get; set; } = ExpressLogType.None;

    /// <summary>
    /// 快递单，可空
    /// </summary>
    [JsonProperty("express")]
    public virtual ExpressInfo? Express { get; set; } = null;

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 物流订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 描述，非空
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 操作会员，可空
    /// </summary>
    [JsonProperty("customer")]
    public virtual CustomerInfo? Customer { get; set; } = null;

    /// <summary>
    /// 操作用户，可空
    /// </summary>
    [JsonProperty("user")]
    public virtual UserInfo? User { get; set; } = null;

}