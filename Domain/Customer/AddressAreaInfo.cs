using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Newtonsoft.Json;

namespace shunda.Domain.Customer;

/// <summary>
/// 地区
/// </summary>
public class AddressAreaInfo : EntityNoCreateNoUpdate
{
    /// <summary>
    /// 城市编码，非空
    /// </summary>
    [Required(ErrorMessage = "城市编码不能为空！")]
    [JsonProperty("city")]
    public virtual AddressCityInfo City { get; set; } = new ();

    /// <summary>
    /// 名称，非空
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50, ErrorMessage = "名称长度不能大于50！")]
    [JsonProperty("name")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 简称，非空
    /// </summary>
    [Required(ErrorMessage = "简称不能为空！")]
    [MaxLength(20, ErrorMessage = "简称长度不能大于20！")]
    [JsonProperty("shortName")]
    public virtual string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// 纬度，非空
    /// </summary>
    [Required(ErrorMessage = "纬度不能为空！")]
    [MaxLength(20, ErrorMessage = "纬度长度不能大于20！")]
    [JsonProperty("lng")]
    public virtual string Lng { get; set; } = string.Empty;

    /// <summary>
    /// 经度，非空
    /// </summary>
    [Required(ErrorMessage = "经度不能为空！")]
    [MaxLength(20, ErrorMessage = "经度长度不能大于20！")]
    [JsonProperty("lat")]
    public virtual string Lat { get; set; } = string.Empty;

    /// <summary>
    /// 排序，非空
    /// </summary>
    [Required(ErrorMessage = "排序不能为空！")]
    [JsonProperty("sort")]
    public virtual int Sort { get; set; } = 0;

}