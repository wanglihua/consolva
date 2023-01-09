using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 运费预估选择记录
/// </summary>
public class FreightEstimateSelectInfo {
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
    /// 物品货值，非空
    /// </summary>
    public virtual decimal GoodsValue { get; set; }

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
    /// 保险，非空
    /// </summary>
    [JsonProperty("insuranceId")]
    public virtual int InsuranceId { get; set; } = 0;

    /// <summary>
    /// GST，非空
    /// </summary>
    [JsonProperty("gstId")]
    public virtual int GstId { get; set; } = 0;
}