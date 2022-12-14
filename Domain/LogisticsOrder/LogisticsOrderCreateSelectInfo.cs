using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单创建选择记录
/// </summary>
public class LogisticsOrderCreateSelectInfo
{
    /// <summary>
    /// 会员ID，非空
    /// </summary>
    [Required(ErrorMessage = "会员ID不能为空！")]
    [JsonProperty("customerId")]
    public virtual int CustomerId { get; set; } = 0;

    /// <summary>
    /// 物品类型，可空
    /// </summary>
    [JsonProperty("goodsTypeId")]
    public virtual int? GoodsTypeId { get; set; } = null;

    /// <summary>
    /// 国家，可空
    /// </summary>
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 头程方式，可空
    /// </summary>
    [JsonProperty("firstType")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType FirstType { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 头程物流，可空
    /// </summary>
    [JsonProperty("firstModeId")]
    public virtual int? FirstModeId { get; set; } = null;

    /// <summary>
    /// 收货地址，可空
    /// </summary>
    [JsonProperty("addressReceiveId")]
    public virtual int? AddressReceiveId { get; set; } = null;

    /// <summary>
    /// 发货地址，可空
    /// </summary>
    [JsonProperty("addressSendId")]
    public virtual int? AddressSendId { get; set; } = null;

    /// <summary>
    /// 保险，可空
    /// </summary>
    [JsonProperty("insuranceId")]
    public virtual int? InsuranceId { get; set; } = null;

    /// <summary>
    /// GST，可空
    /// </summary>
    [JsonProperty("gstId")]
    public virtual int? GstId { get; set; } = null;

    /// <summary>
    /// 优惠券id，可空
    /// </summary>
    [JsonProperty("couponId")]
    public virtual int? CouponId { get; set; } = null;

    /// <summary>
    /// 总费用，可空
    /// </summary>
    [JsonProperty("totalFee")]
    public virtual decimal? TotalFee { get; set; } = null;

    /// <summary>
    /// 数据是否有效，非空
    /// </summary>
    [Required(ErrorMessage = "数据是否有效不能为空！")]
    [JsonProperty("valid")]
    public virtual bool Valid { get; set; } = false;

}