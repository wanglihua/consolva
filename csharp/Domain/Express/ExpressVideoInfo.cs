using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单视频
/// </summary>
public class ExpressVideoInfo : Entity
{
    /// <summary>
    /// 客户，可空
    /// </summary>
    [JsonProperty("customer")]
    public virtual CustomerInfo? Customer { get; set; } = null;

    /// <summary>
    /// 物流订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    [JsonProperty("orderCode")]
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号，可空
    /// </summary>
    [JsonProperty("express")]
    public virtual ExpressInfo? Express { get; set; } = null;

    /// <summary>
    /// 快递单号，可空
    /// </summary>
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string? TrackingNo { get; set; } = null;

    /// <summary>
    /// 快递公司，可空
    /// </summary>
    [MaxLength(100, ErrorMessage = "快递公司长度不能大于100！")]
    [JsonProperty("expressComName")]
    public virtual string? ExpressComName { get; set; } = null;

    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(ExpressVideoTypeModelBinder))]
    public virtual ExpressVideoType Type { get; set; } = ExpressVideoType.None;

    /// <summary>
    /// 存储路径，非空
    /// </summary>
    [Required(ErrorMessage = "存储路径不能为空！")]
    [MaxLength(255, ErrorMessage = "存储路径长度不能大于255！")]
    [JsonProperty("storePath")]
    public virtual string StorePath { get; set; } = string.Empty;

    /// <summary>
    /// 存储名称，非空
    /// </summary>
    [Required(ErrorMessage = "存储名称不能为空！")]
    [MaxLength(50, ErrorMessage = "存储名称长度不能大于50！")]
    [JsonProperty("storeName")]
    public virtual string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 上传用户，可空
    /// </summary>
    [JsonProperty("user")]
    public virtual UserInfo? User { get; set; } = null;

    /// <summary>
    /// 存储位置，非空
    /// </summary>
    [Required(ErrorMessage = "存储位置不能为空！")]
    [JsonProperty("storeLoc")]
    [ModelBinder(typeof(ExpressPhotoStoreLocModelBinder))]
    public virtual ExpressPhotoStoreLoc StoreLoc { get; set; } = ExpressPhotoStoreLoc.None;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ExpressVideoStatusModelBinder))]
    public virtual ExpressVideoStatus Status { get; set; } = ExpressVideoStatus.None;

}