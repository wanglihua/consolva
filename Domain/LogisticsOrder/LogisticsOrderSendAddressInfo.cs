using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Customer;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单发货地址
/// </summary>
public class LogisticsOrderSendAddressInfo : Entity
{
    /// <summary>
    /// 订单id，非空
    /// </summary>
    [Required(ErrorMessage = "订单id不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new ();

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [JsonProperty("country")]
    [ModelBinder(typeof(CountryModelBinder))]
    public virtual Country Country { get; set; } = Country.None;

    /// <summary>
    /// 发货人名，非空
    /// </summary>
    [Required(ErrorMessage = "发货人名不能为空！")]
    [MaxLength(60, ErrorMessage = "发货人名长度不能大于60！")]
    [JsonProperty("firstName")]
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 发货人姓，非空
    /// </summary>
    [Required(ErrorMessage = "发货人姓不能为空！")]
    [MaxLength(60, ErrorMessage = "发货人姓长度不能大于60！")]
    [JsonProperty("lastName")]
    public virtual string LastName { get; set; } = string.Empty;

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
    public virtual AddressProvinceInfo Province { get; set; } = new ();

    /// <summary>
    /// 城市代码，非空
    /// </summary>
    [Required(ErrorMessage = "城市代码不能为空！")]
    [JsonProperty("city")]
    public virtual AddressCityInfo City { get; set; } = new ();

    /// <summary>
    /// 区域代码，非空
    /// </summary>
    [Required(ErrorMessage = "区域代码不能为空！")]
    [JsonProperty("area")]
    public virtual AddressAreaInfo Area { get; set; } = new ();

    /// <summary>
    /// 街道代码，非空
    /// </summary>
    [Required(ErrorMessage = "街道代码不能为空！")]
    [JsonProperty("street")]
    public virtual AddressStreetInfo Street { get; set; } = new ();

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

}