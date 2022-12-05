using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 货架用户关联
/// </summary>
public class ShelfUserInfo : Entity
{
    /// <summary>
    /// 用户，非空
    /// </summary>
    [Required(ErrorMessage = "用户不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    [JsonProperty("warehouse")]
    public virtual WarehouseInfo Warehouse { get; set; } = new ();

    /// <summary>
    /// 货架，非空
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    [JsonProperty("shelf")]
    public virtual ShelfInfo Shelf { get; set; } = new ();

    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(ShelfUserTypeModelBinder))]
    public virtual ShelfUserType Type { get; set; } = ShelfUserType.None;

}