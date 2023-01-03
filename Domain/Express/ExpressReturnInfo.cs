using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 快递退运
/// </summary>
public class ExpressReturnInfo : Entity {
    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ExpressInfo();

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 入库跟踪号，非空
    /// </summary>
    [Required(ErrorMessage = "入库跟踪号不能为空！")]
    [MaxLength(20, ErrorMessage = "入库跟踪号长度不能大于20！")]
    [JsonProperty("checkinTrackingNo")]
    public virtual string CheckinTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递公司，非空
    /// </summary>
    [Required(ErrorMessage = "快递公司不能为空！")]
    [MaxLength(100, ErrorMessage = "快递公司长度不能大于100！")]
    [JsonProperty("expressCompany")]
    public virtual string ExpressCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 入库时间，非空
    /// </summary>
    [Required(ErrorMessage = "入库时间不能为空！")]
    [JsonProperty("addTime")]
    public virtual DateTime AddTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 添加用户，非空
    /// </summary>
    [Required(ErrorMessage = "添加用户不能为空！")]
    [JsonProperty("addUser")]
    public virtual UserInfo AddUser { get; set; } = new UserInfo();

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    [JsonProperty("warehouse")]
    public virtual WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    /// <summary>
    /// 客户，非空
    /// </summary>
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ExpressStatusModelBinder))]
    public virtual ExpressStatus Status { get; set; } = ExpressStatus.None;

    /// <summary>
    /// 退运快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "退运快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "退运快递单号长度不能大于30！")]
    [JsonProperty("returnTrackingNo")]
    public virtual string ReturnTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 退运快递公司，非空
    /// </summary>
    [Required(ErrorMessage = "退运快递公司不能为空！")]
    [MaxLength(100, ErrorMessage = "退运快递公司长度不能大于100！")]
    [JsonProperty("returnExpressCompany")]
    public virtual string ReturnExpressCompany { get; set; } = string.Empty;

    /// <summary>
    /// 退运申请时间，非空
    /// </summary>
    [Required(ErrorMessage = "退运申请时间不能为空！")]
    [JsonProperty("returnApplyTime")]
    public virtual DateTime ReturnApplyTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 退运取件码，非空
    /// </summary>
    [Required(ErrorMessage = "退运取件码不能为空！")]
    [MaxLength(20, ErrorMessage = "退运取件码长度不能大于20！")]
    [JsonProperty("returnPickupCode")]
    public virtual string ReturnPickupCode { get; set; } = string.Empty;

    /// <summary>
    /// 退运操作人，可空
    /// </summary>
    [JsonProperty("returnUser")]
    public virtual CustomerInfo? ReturnUser { get; set; } = null;

    /// <summary>
    /// 退运时间，可空
    /// </summary>
    [JsonProperty("returnTime")]
    public virtual DateTime? ReturnTime { get; set; } = null;
}