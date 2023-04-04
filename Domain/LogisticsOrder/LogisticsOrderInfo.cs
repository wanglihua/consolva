using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.Storage;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单
/// </summary>
public class LogisticsOrderInfo : Entity {
    /// <summary>
    /// 订单编号，非空
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    [JsonProperty("code")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// 客户ID，非空
    /// </summary>
    [Required(ErrorMessage = "客户ID不能为空！")]
    [MaxLength(30, ErrorMessage = "客户ID长度不能大于30！")]
    [JsonProperty("customerCode")]
    public virtual string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 订单收费方式，非空
    /// </summary>
    [Required(ErrorMessage = "订单收费方式不能为空！")]
    [JsonProperty("feeMethod")]
    [ModelBinder(typeof(LogisticsOrderFeeMethodModelBinder))]
    public virtual LogisticsOrderFeeMethod FeeMethod { get; set; } = LogisticsOrderFeeMethod.None;

    /// <summary>
    /// 创建方式，非空
    /// </summary>
    [Required(ErrorMessage = "创建方式不能为空！")]
    [JsonProperty("createMethod")]
    [ModelBinder(typeof(LogisticsOrderCreateMethodModelBinder))]
    public virtual LogisticsOrderCreateMethod CreateMethod { get; set; } = LogisticsOrderCreateMethod.None;

    /// <summary>
    /// 收货仓库，非空
    /// </summary>
    [Required(ErrorMessage = "收货仓库不能为空！")]
    [JsonProperty("warehouse")]
    public virtual WarehouseInfo Warehouse { get; set; } = new();

    /// <summary>
    /// 物流方式类型，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式类型不能为空！")]
    [JsonProperty("firstModeType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstModeType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 物流方式，非空
    /// 这是一个主的，客户选择的物流方式。实际可能一个订单会多个物流方式。
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    [JsonProperty("firstMode")]
    public virtual LogisticsModeInfo FirstMode { get; set; } = new();

    /// <summary>
    /// 保险id，非空
    /// </summary>
    [Required(ErrorMessage = "保险id不能为空！")]
    [JsonProperty("insurance")]
    public virtual LogisticsInsuranceInfo Insurance { get; set; } = new();

    /// <summary>
    /// 纳税申报，非空
    /// </summary>
    [Required(ErrorMessage = "纳税申报不能为空！")]
    [JsonProperty("gst")]
    public virtual LogisticsGstInfo Gst { get; set; } = new();

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 订单状态，非空
    /// </summary>
    [Required(ErrorMessage = "订单状态不能为空！")]
    [JsonProperty("orderStatus")]
    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public virtual LogisticsOrderStatus OrderStatus { get; set; } = LogisticsOrderStatus.None;

    /// <summary>
    /// 支付状态，非空
    /// </summary>
    [Required(ErrorMessage = "支付状态不能为空！")]
    [JsonProperty("payStatus")]
    [ModelBinder(typeof(PayStatusModelBinder))]
    public virtual PayStatus PayStatus { get; set; } = PayStatus.None;

    /// <summary>
    /// 支付方式，可空
    /// </summary>
    [JsonProperty("payType")]
    [ModelBinder(typeof(PayTypeModelBinder))]
    public virtual PayType PayType { get; set; } = PayType.None;

    /// <summary>
    /// 商品总价，非空
    /// </summary>
    [Required(ErrorMessage = "商品总价不能为空！")]
    [JsonProperty("goodsValue")]
    public virtual decimal GoodsValue { get; set; } = 0m;

    /// <summary>
    /// 应付款金额，非空
    /// </summary>
    [Required(ErrorMessage = "应付款金额不能为空！")]
    [JsonProperty("orderAmount")]
    public virtual decimal OrderAmount { get; set; } = 0m;

    /// <summary>
    /// 已支付金额，非空
    /// </summary>
    [Required(ErrorMessage = "已支付金额不能为空！")]
    [JsonProperty("paidAmount")]
    public virtual decimal PaidAmount { get; set; } = 0m;

    /// <summary>
    /// 欠费金额，通过 NHibernate 公式映射实现
    /// </summary>
    public virtual decimal ArrearsAmountFormula { get; protected set; } = 0;

    /// <summary>
    /// 打包类型，可空
    /// </summary>
    [JsonProperty("repackType")]
    [ModelBinder(typeof(LogisticsRepackTypeModelBinder))]
    public virtual LogisticsRepackType RepackType { get; set; } = LogisticsRepackType.None;

    /// <summary>
    /// 仓位，非空
    /// </summary>
    [JsonProperty("storageLoc")]
    public virtual StorageLocInfo StorageLoc { get; set; } = new();

    /// <summary>
    /// 占用仓位数，可空
    /// </summary>
    [JsonProperty("storageLocCount")]
    public virtual int StorageLocCount { get; set; }

    /// <summary>
    /// 取件码，非空
    /// </summary>
    [Required(ErrorMessage = "取件码不能为空！")]
    [MaxLength(12, ErrorMessage = "取件码长度不能大于12！")]
    [JsonProperty("pickupCode")]
    public virtual string PickupCode { get; set; } = string.Empty;

    /// <summary>
    /// 支付时间，可空
    /// </summary>
    [JsonProperty("payTime")]
    public virtual DateTime? PayTime { get; set; } = null;

    /// <summary>
    /// 提交打包时间，可空
    /// </summary>
    [JsonProperty("submitPackTime")]
    public virtual DateTime? SubmitPackTime { get; set; } = null;

    /// <summary>
    /// 完成打包时间，可空
    /// </summary>
    [JsonProperty("completePackTime")]
    public virtual DateTime? CompletePackTime { get; set; } = null;

    /// <summary>
    /// 转运时间，可空
    /// </summary>
    [JsonProperty("transferTime")]
    public virtual DateTime? TransferTime { get; set; } = null;

    /// <summary>
    /// 签收时间，可空
    /// </summary>
    [JsonProperty("deliverTime")]
    public virtual DateTime? DeliverTime { get; set; } = null;

    /// <summary>
    /// 用于 NHibernate 查询生成 SQL
    /// </summary>
    [BindNever]
    public virtual IList<LogisticsOrderPackScheduleInfo> PackSchedules { get; set; } = new List<LogisticsOrderPackScheduleInfo>();

    #region Cache

    /// <summary>
    /// 列表颜色
    /// </summary>
    public virtual string ListColorCache { get; set; } = string.Empty;

    /// <summary>
    /// 打包汇总数据
    /// </summary>
    public virtual LogisticsOrderPackDataInfo? PackDataCache { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    public virtual string OrderRemarkCache { get; set; } = string.Empty;

    /// <summary>
    /// 打包排班 打包工用户名
    /// </summary>
    public virtual string PackScheduleUserNameCache { get; set; } = string.Empty;

    /// <summary>
    /// 物流方式名称，用于列表显示
    /// 如果还没打包就取订单创建物流方式
    /// 如果打包后只有一个物流方式，就取打包后的物流方式
    /// 如果打包后有多个物流方式，就取订单创建物流方式
    /// </summary>
    public virtual string FirstModeNameCache { get; set; } = string.Empty;

    #endregion
}