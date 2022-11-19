using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 商品快递
/// </summary>
public class ExpressInfo : Entity
{
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
    /// 快递单描述，非空
    /// </summary>
    [Required(ErrorMessage = "快递单描述不能为空！")]
    [MaxLength(500, ErrorMessage = "快递单描述长度不能大于500！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

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
    /// 面单客户，非空
    /// </summary>
    [Required(ErrorMessage = "面单客户不能为空！")]
    [MaxLength(50, ErrorMessage = "面单客户长度不能大于50！")]
    [JsonProperty("billCustomerCode")]
    public virtual string BillCustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 面单客户Id，非空
    /// </summary>
    [Required(ErrorMessage = "面单客户Id不能为空！")]
    [JsonProperty("billCustomerId")]
    public virtual int BillCustomerId { get; set; } = 0;

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    [JsonProperty("warehouse")]
    public virtual WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    /// <summary>
    /// 货架，非空
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    [JsonProperty("shelf")]
    public virtual ShelfInfo Shelf { get; set; } = new ShelfInfo();

    /// <summary>
    /// 库位，非空
    /// </summary>
    [Required(ErrorMessage = "库位不能为空！")]
    [JsonProperty("storageLoc")]
    public virtual StorageLocInfo StorageLoc { get; set; } = new StorageLocInfo();

    /// <summary>
    /// 包裹数量，非空
    /// </summary>
    [Required(ErrorMessage = "包裹数量不能为空！")]
    [JsonProperty("packageCount")]
    public virtual int PackageCount { get; set; } = 0;

    /// <summary>
    /// 认领时间，可空
    /// </summary>
    [JsonProperty("claimTime")]
    public virtual DateTime? ClaimTime { get; set; } = null;

    /// <summary>
    /// 客户忽略，非空
    /// </summary>
    [Required(ErrorMessage = "客户忽略不能为空！")]
    [JsonProperty("customerIgnored")]
    public virtual bool CustomerIgnored { get; set; } = false;

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
    /// 客户，可空
    /// </summary>
    [JsonProperty("customer")]
    public virtual CustomerInfo? Customer { get; set; } = null;

    /// <summary>
    /// 头程方式，非空
    /// </summary>
    [Required(ErrorMessage = "头程方式不能为空！")]
    [JsonProperty("firstType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 头程物流，可空
    /// </summary>
    [JsonProperty("firstMode")]
    public virtual LogisticsModeInfo? FirstMode { get; set; } = null;

    /// <summary>
    /// 提交打包时间，可空
    /// </summary>
    [JsonProperty("submitPackTime")]
    public virtual DateTime? SubmitPackTime { get; set; } = null;

    /// <summary>
    /// 总重量，非空
    /// </summary>
    [Required(ErrorMessage = "总重量不能为空！")]
    [JsonProperty("totalWeight")]
    public virtual decimal TotalWeight { get; set; } = 0m;

    /// <summary>
    /// 总体积，非空
    /// </summary>
    [Required(ErrorMessage = "总体积不能为空！")]
    [JsonProperty("totalVolume")]
    public virtual decimal TotalVolume { get; set; } = 0m;

    /// <summary>
    /// 上架时间，可空
    /// </summary>
    [JsonProperty("onShelfTime")]
    public virtual DateTime? OnShelfTime { get; set; } = null;

    /// <summary>
    /// 上架用户，可空
    /// </summary>
    [JsonProperty("onShelfUser")]
    public virtual UserInfo? OnShelfUser { get; set; } = null;

    /// <summary>
    /// 下架时间，可空
    /// </summary>
    [JsonProperty("offShelfTime")]
    public virtual DateTime? OffShelfTime { get; set; } = null;

    /// <summary>
    /// 下架用户，可空
    /// </summary>
    [JsonProperty("offShelfUser")]
    public virtual UserInfo? OffShelfUser { get; set; } = null;

    /// <summary>
    /// 入流动仓时间，可空
    /// </summary>
    [JsonProperty("packingTime")]
    public virtual DateTime? PackingTime { get; set; } = null;

    /// <summary>
    /// 入流动仓用户，可空
    /// </summary>
    [JsonProperty("packingUser")]
    public virtual UserInfo? PackingUser { get; set; } = null;

    /// <summary>
    /// 快递单货架状态，非空
    /// </summary>
    [Required(ErrorMessage = "快递单货架状态不能为空！")]
    [JsonProperty("shelfStatus")]
    [ModelBinder(typeof(ExpressShelfStatusModelBinder))]
    public virtual ExpressShelfStatus ShelfStatus { get; set; } = ExpressShelfStatus.None;

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
    /// 退运申请时间，可空
    /// </summary>
    [JsonProperty("returnApplyTime")]
    public virtual DateTime? ReturnApplyTime { get; set; } = null;

    /// <summary>
    /// 退运取件码，可空
    /// </summary>
    [MaxLength(20, ErrorMessage = "退运取件码长度不能大于20！")]
    [JsonProperty("returnPickupCode")]
    public virtual string? ReturnPickupCode { get; set; } = null;

    /// <summary>
    /// 退运操作人，可空
    /// </summary>
    [JsonProperty("returnUserId")]
    public virtual int? ReturnUserId { get; set; } = null;

    /// <summary>
    /// 退运时间，可空
    /// </summary>
    [JsonProperty("returnTime")]
    public virtual DateTime? ReturnTime { get; set; } = null;

    /// <summary>
    /// 堆存用户，可空
    /// </summary>
    [JsonProperty("dumpUser")]
    public virtual UserInfo? DumpUser { get; set; } = null;

    /// <summary>
    /// 堆存时间，可空
    /// </summary>
    [JsonProperty("dumpTime")]
    public virtual DateTime? DumpTime { get; set; } = null;

    /// <summary>
    /// 堆存状态，非空
    /// </summary>
    [Required(ErrorMessage = "堆存状态不能为空！")]
    [JsonProperty("dumpStatus")]
    [ModelBinder(typeof(ExpressDumpStatusModelBinder))]
    public virtual ExpressDumpStatus DumpStatus { get; set; } = ExpressDumpStatus.None;

    /// <summary>
    /// 归档类型，非空
    /// </summary>
    [Required(ErrorMessage = "归档类型不能为空！")]
    [JsonProperty("archiveType")]
    [ModelBinder(typeof(ExpressArchiveTypeModelBinder))]
    public virtual ExpressArchiveType ArchiveType { get; set; } = ExpressArchiveType.None;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(ExpressStatusModelBinder))]
    public virtual ExpressStatus Status { get; set; } = ExpressStatus.None;

    #region cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    public virtual IList<ExpressSizeWeightInfo> ExpressSizeWeightListCache { get; set; } = new List<ExpressSizeWeightInfo>();

    public virtual IList<ExpressPhotoInfo> ExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    public virtual ExpressExpectInfo? ExpressExpectCache { get; set; } = null;

    #endregion
}