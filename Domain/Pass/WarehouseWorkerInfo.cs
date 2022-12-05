using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Storage;

namespace shunda.Domain.Pass;

/// <summary>
/// 仓库工作人员
/// </summary>
public class WarehouseWorkerInfo : Entity
{
    /// <summary>
    /// 工作人员类型，非空
    /// </summary>
    [Required(ErrorMessage = "工作人员类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(WarehouseWorkerTypeModelBinder))]
    public virtual WarehouseWorkerType Type { get; set; } = WarehouseWorkerType.None;

    /// <summary>
    /// 仓库，非空
    /// </summary>
    [Required(ErrorMessage = "仓库不能为空！")]
    [JsonProperty("warehouse")]
    public virtual WarehouseInfo Warehouse { get; set; } = new ();

    /// <summary>
    /// 用户，非空
    /// </summary>
    [Required(ErrorMessage = "用户不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 描述，非空
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(300, ErrorMessage = "描述长度不能大于300！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

}