using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单号入库用户货架选择记忆
/// </summary>
public class ExpressCheckinUserShelfSelectInfo : Entity
{
    /// <summary>
    /// 用户，非空
    /// </summary>
    [Required(ErrorMessage = "用户不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 货架，非空
    /// </summary>
    [Required(ErrorMessage = "货架不能为空！")]
    [JsonProperty("shelf")]
    public virtual ShelfInfo Shelf { get; set; } = new ();

    /// <summary>
    /// 仓位规格，非空
    /// </summary>
    [Required(ErrorMessage = "仓位规格不能为空！")]
    [JsonProperty("storageLocSpec")]
    public virtual StorageLocSpecInfo StorageLocSpec { get; set; } = new ();
}