using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单创建选择记录
/// </summary>
public class LogisticsOrderCreateSelectInfo {
    /// <summary>
    /// 会员ID，非空
    /// 主键
    /// </summary>
    [Required(ErrorMessage = "会员ID不能为空！")]
    [JsonProperty("customerId")]
    public virtual int CustomerId { get; set; } = 0;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [JsonProperty("goodsType")]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 国家，非空
    /// </summary>
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 头程方式，非空
    /// </summary>
    [JsonProperty("firstType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 头程物流，非空
    /// </summary>
    [JsonProperty("firstModeId")]
    public virtual int FirstModeId { get; set; } = 0;

    /// <summary>
    /// 收货地址，非空
    /// </summary>
    [JsonProperty("addressReceiveId")]
    public virtual int AddressReceiveId { get; set; } = 0;

    /// <summary>
    /// 发货地址，非空
    /// </summary>
    [JsonProperty("addressSendId")]
    public virtual int AddressSendId { get; set; } = 0;

    /// <summary>
    /// 保险，非空
    /// </summary>
    [JsonProperty("insuranceId")]
    public virtual int InsuranceId { get; set; } = 0;

    /// <summary>
    /// GST，非空
    /// </summary>
    [JsonProperty("gstId")]
    public virtual int GstId { get; set; } = 0;

    /// <summary>
    /// 优惠券id，非空
    /// 这个基本上不用了，优惠券在支付时指定好了。
    /// 创建时不计算订单金额的话，创建时指定优惠券就没有任何意义
    /// </summary>
    [JsonProperty("couponId")]
    public virtual int CouponId { get; set; } = 0;

    /// <summary>
    /// 总费用，非空
    /// </summary>
    [JsonProperty("totalFee")]
    public virtual decimal TotalFee { get; set; } = 0.0m;

    /// <summary>
    /// 数据是否有效，非空
    /// </summary>
    [Required(ErrorMessage = "数据是否有效不能为空！")]
    [JsonProperty("valid")]
    public virtual bool Valid { get; set; } = false;
}