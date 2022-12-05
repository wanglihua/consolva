using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递堆存排班
/// </summary>
public class ExpressDumpScheduleInfo : Entity
{
    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ();

    /// <summary>
    /// 堆存人，非空
    /// </summary>
    [Required(ErrorMessage = "堆存人不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 堆存类型，非空
    /// </summary>
    [Required(ErrorMessage = "堆存类型不能为空！")]
    [JsonProperty("dumpType")]
    [ModelBinder(typeof(ExpressDumpTypeModelBinder))]
    public virtual ExpressDumpType DumpType { get; set; } = ExpressDumpType.None;

}