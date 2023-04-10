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
public class LogisticsOrderListViewInfo : Entity {
    /// <summary>
    /// 订单编号，非空
    /// </summary>
    [JsonProperty("code")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客户，非空
    /// </summary>
    [JsonProperty("customerId")]
    public virtual int CustomerId { get; set; } = new();

    /// <summary>
    /// 客户性质
    /// </summary>
    [JsonProperty("customerCharacter")]
    public virtual string CustomerCharacter { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID，非空
    /// </summary>
    [JsonProperty("customerCode")]
    public virtual string CustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 订单状态，非空
    /// </summary>
    [JsonProperty("orderStatus")]
    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public virtual LogisticsOrderStatus OrderStatus { get; set; } = LogisticsOrderStatus.None;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 商品总价，非空
    /// </summary>
    [Required(ErrorMessage = "商品总价不能为空！")]
    [JsonProperty("goodsValue")]
    public virtual decimal GoodsValue { get; set; }

    /// <summary>
    /// 应付款金额，非空
    /// </summary>
    [Required(ErrorMessage = "应付款金额不能为空！")]
    [JsonProperty("orderAmount")]
    public virtual decimal OrderAmount { get; set; }

    /// <summary>
    /// 欠费金额
    /// </summary>
    public virtual decimal ArrearsAmount { get; set; }

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
    /// 国家，非空
    /// </summary>
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 收货仓库，非空
    /// </summary>
    [JsonProperty("warehouseId")]
    public virtual int WarehouseId { get; set; }

    [JsonProperty("warehouseName")]
    public virtual string WarehouseName { get; set; } = string.Empty;

    /// <summary>
    /// 物流方式类型，非空
    /// </summary>
    [JsonProperty("firstModeType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstModeType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [JsonProperty("firstModeId")]
    public virtual int FirstModeId { get; set; }

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [JsonProperty("firstModeCode")]
    public virtual string FirstModeCode { get; set; } = string.Empty;    

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [JsonProperty("firstModeName")]
    public virtual string FirstModeName { get; set; } = string.Empty;

    /// <summary>
    /// 支付时间，可空
    /// </summary>
    [JsonProperty("payTime")]
    public virtual DateTime? PayTime { get; set; }

    /// <summary>
    /// 提交打包时间，可空
    /// </summary>
    [JsonProperty("submitPackTime")]
    public virtual DateTime? SubmitPackTime { get; set; }

    /// <summary>
    /// 完成打包时间，可空
    /// </summary>
    [JsonProperty("completePackTime")]
    public virtual DateTime? CompletePackTime { get; set; }

    /// <summary>
    /// 转运时间，可空
    /// </summary>
    [JsonProperty("transferTime")]
    public virtual DateTime? TransferTime { get; set; }

    /// <summary>
    /// 签收时间，可空
    /// </summary>
    [JsonProperty("deliverTime")]
    public virtual DateTime? DeliverTime { get; set; }

    /// <summary>
    /// 体积重 kg，非空
    /// </summary>
    [JsonProperty("totalVolumeWeight")]
    public virtual decimal? TotalVolumeWeight { get; set; }

    /// <summary>
    /// 实际重量 kg，非空
    /// </summary>
    [JsonProperty("totalActualWeight")]
    public virtual decimal? TotalActualWeight { get; set; }

    /// <summary>
    /// 最长边 cm，非空
    /// </summary>
    [JsonProperty("maxSideLength")]
    public virtual decimal? MaxSideLength { get; set; }

    /// <summary>
    /// 总箱数，非空
    /// </summary>
    [JsonProperty("totalPackageCount")]
    public virtual int? TotalPackPackageCount { get; set; }

    #region Cache

    /// <summary>
    /// 列表颜色
    /// </summary>
    public virtual string ListColorCache { get; set; } = string.Empty;

    /// <summary>
    /// 订单备注
    /// </summary>
    public virtual string OrderRemarkCache { get; set; } = string.Empty;

    /// <summary>
    /// 物流方式名称，用于列表显示
    /// 如果还没打包就取订单创建物流方式
    /// 如果打包后只有一个物流方式，就取打包后的物流方式
    /// 如果打包后有多个物流方式，就取订单创建物流方式
    /// </summary>
    public virtual string FirstModeNameCache { get; set; } = string.Empty;

    #endregion    
}