using System.ComponentModel.DataAnnotations;
using shunda.Domain.Express;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Express.Models.ExpressReturn;

public class ExpressReturnSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int ExpressReturnId { get; set; }

    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    public int ExpressId { get; set; } = 0;

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    public string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 入库跟踪号，非空
    /// </summary>
    [Required(ErrorMessage = "入库跟踪号不能为空！")]
    [MaxLength(20, ErrorMessage = "入库跟踪号长度不能大于20！")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递公司，非空
    /// </summary>
    [Required(ErrorMessage = "快递公司不能为空！")]
    [MaxLength(100, ErrorMessage = "快递公司长度不能大于100！")]
    public string ExpressCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 入库时间，非空
    /// </summary>
    [Required(ErrorMessage = "入库时间不能为空！")]
    public DateTime AddTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 添加用户，非空
    /// </summary>
    [Required(ErrorMessage = "添加用户不能为空！")]
    public int AddUserId { get; set; } = 0;

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    public int WarehouseId { get; set; } = 0;

    /// <summary>
    /// 客户，可空
    /// </summary>
    public int? CustomerId { get; set; } = null;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(ExpressStatusModelBinder))]
    public ExpressStatus Status { get; set; } = ExpressStatus.None;

    /// <summary>
    /// 退运快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "退运快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "退运快递单号长度不能大于30！")]
    public string ReturnTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 退运快递公司，非空
    /// </summary>
    [Required(ErrorMessage = "退运快递公司不能为空！")]
    [MaxLength(100, ErrorMessage = "退运快递公司长度不能大于100！")]
    public string ReturnExpressCompany { get; set; } = string.Empty;

    /// <summary>
    /// 退运申请时间，非空
    /// </summary>
    [Required(ErrorMessage = "退运申请时间不能为空！")]
    public DateTime ReturnApplyTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 退运取件码，可空
    /// </summary>
    [MaxLength(20, ErrorMessage = "退运取件码长度不能大于20！")]
    public string? ReturnPickupCode { get; set; } = null;

    /// <summary>
    /// 退运操作人，可空
    /// </summary>
    public int? ReturnUserId { get; set; } = null;

    /// <summary>
    /// 退运时间，可空
    /// </summary>
    public DateTime? ReturnTime { get; set; } = null;

}