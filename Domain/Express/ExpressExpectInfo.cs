using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单号预报
/// </summary>
public class ExpressExpectInfo : Entity
{
    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 备注，非空
    /// </summary>
    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(500, ErrorMessage = "备注长度不能大于500！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new ();

    /// <summary>
    /// 客户最后编辑时间，非空
    /// </summary>
    [Required(ErrorMessage = "客户最后编辑时间不能为空！")]
    [JsonProperty("lastEditTime")]
    public virtual DateTime LastEditTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ExpressExpectStatusModelBinder))]
    public virtual ExpressExpectStatus Status { get; set; } = ExpressExpectStatus.None;

    /// <summary>
    /// 超期未入库消息发送时间，可空
    /// </summary>
    [JsonProperty("overdueMsgSent")]
    public virtual DateTime? OverdueMsgSent { get; set; } = null;

    #region Cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}