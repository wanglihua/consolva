using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单收货地址
/// </summary>
public class LogisticsOrderReceiveAddressInfo : Entity
{
    /// <summary>
    /// 客户id，非空
    /// </summary>
    [Required(ErrorMessage = "客户id不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new ();

    /// <summary>
    /// 收货人名，非空
    /// </summary>
    [Required(ErrorMessage = "收货人名不能为空！")]
    [MaxLength(60, ErrorMessage = "收货人名长度不能大于60！")]
    [JsonProperty("firstName")]
    public virtual string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓，非空
    /// </summary>
    [Required(ErrorMessage = "收货人姓不能为空！")]
    [MaxLength(60, ErrorMessage = "收货人姓长度不能大于60！")]
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
    /// 省份，非空
    /// </summary>
    [Required(ErrorMessage = "省份不能为空！")]
    [MaxLength(50, ErrorMessage = "省份长度不能大于50！")]
    [JsonProperty("state")]
    public virtual string State { get; set; } = string.Empty;

    /// <summary>
    /// 城市，非空
    /// </summary>
    [Required(ErrorMessage = "城市不能为空！")]
    [MaxLength(50, ErrorMessage = "城市长度不能大于50！")]
    [JsonProperty("city")]
    public virtual string City { get; set; } = string.Empty;

    /// <summary>
    /// 地址，非空
    /// </summary>
    [Required(ErrorMessage = "地址不能为空！")]
    [MaxLength(200, ErrorMessage = "地址长度不能大于200！")]
    [JsonProperty("address")]
    public virtual string Address { get; set; } = string.Empty;

    /// <summary>
    /// Unit号，非空
    /// </summary>
    [Required(ErrorMessage = "Unit号不能为空！")]
    [MaxLength(20, ErrorMessage = "Unit号长度不能大于20！")]
    [JsonProperty("unitLoc")]
    public virtual string UnitLoc { get; set; } = string.Empty;

    /// <summary>
    /// 邮政编码，非空
    /// </summary>
    [Required(ErrorMessage = "邮政编码不能为空！")]
    [MaxLength(10, ErrorMessage = "邮政编码长度不能大于10！")]
    [JsonProperty("zipcode")]
    public virtual string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// ABN/ACN，非空
    /// </summary>
    [Required(ErrorMessage = "ABN/ACN不能为空！")]
    [MaxLength(100, ErrorMessage = "ABN/ACN长度不能大于100！")]
    [JsonProperty("tfn")]
    public virtual string Tfn { get; set; } = string.Empty;

    /// <summary>
    /// 公司，非空
    /// </summary>
    [Required(ErrorMessage = "公司不能为空！")]
    [MaxLength(100, ErrorMessage = "公司长度不能大于100！")]
    [JsonProperty("company")]
    public virtual string Company { get; set; } = string.Empty;

}