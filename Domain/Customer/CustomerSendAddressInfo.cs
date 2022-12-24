using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Customer;

/// <summary>
/// 用户发货地址
/// </summary>
public class CustomerSendAddressInfo : Entity {
    /// <summary>
    /// 客户id，非空
    /// </summary>
    [Required(ErrorMessage = "客户id不能为空！")]
    [JsonProperty("customer")]
    public virtual CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 发货人姓名，非空
    /// </summary>
    [Required(ErrorMessage = "发货人姓名不能为空！")]
    [MaxLength(60, ErrorMessage = "发货人姓名长度不能大于60！")]
    [JsonProperty("name")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 手机，非空
    /// </summary>
    [Required(ErrorMessage = "手机不能为空！")]
    [MaxLength(60, ErrorMessage = "手机长度不能大于60！")]
    [JsonProperty("mobile")]
    public virtual string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 省代码，非空
    /// </summary>
    [Required(ErrorMessage = "省代码不能为空！")]
    [JsonProperty("province")]
    public virtual AddressProvinceInfo Province { get; set; } = new();

    /// <summary>
    /// 城市代码，非空
    /// </summary>
    [Required(ErrorMessage = "城市代码不能为空！")]
    [JsonProperty("city")]
    public virtual AddressCityInfo City { get; set; } = new();

    /// <summary>
    /// 区域代码，非空
    /// </summary>
    [Required(ErrorMessage = "区域代码不能为空！")]
    [JsonProperty("area")]
    public virtual AddressAreaInfo Area { get; set; } = new();

    /// <summary>
    /// 街道代码，非空
    /// </summary>
    [Required(ErrorMessage = "街道代码不能为空！")]
    [JsonProperty("street")]
    public virtual AddressStreetInfo Street { get; set; } = new();

    /// <summary>
    /// 地址，非空
    /// </summary>
    [Required(ErrorMessage = "地址不能为空！")]
    [MaxLength(200, ErrorMessage = "地址长度不能大于200！")]
    [JsonProperty("address")]
    public virtual string Address { get; set; } = string.Empty;

    /// <summary>
    /// 邮政编码，非空
    /// </summary>
    [Required(ErrorMessage = "邮政编码不能为空！")]
    [MaxLength(10, ErrorMessage = "邮政编码长度不能大于10！")]
    [JsonProperty("zipcode")]
    public virtual string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// 税号，可空
    /// </summary>
    [MaxLength(100, ErrorMessage = "税号长度不能大于100！")]
    [JsonProperty("tfn")]
    public virtual string? Tfn { get; set; } = null;

    /// <summary>
    /// 公司，可空
    /// </summary>
    [MaxLength(100, ErrorMessage = "公司长度不能大于100！")]
    [JsonProperty("company")]
    public virtual string? Company { get; set; } = null;

    /// <summary>
    /// 默认收货地址，非空
    /// </summary>
    [Required(ErrorMessage = "默认收货地址不能为空！")]
    [JsonProperty("isDefault")]
    public virtual bool IsDefault { get; set; } = false;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(CustomerAddressStatusModelBinder))]
    public virtual CustomerAddressStatus Status { get; set; } = CustomerAddressStatus.None;

    #region cache

    public virtual string CustomerCodeCache { get; set; } = string.Empty;

    #endregion
}