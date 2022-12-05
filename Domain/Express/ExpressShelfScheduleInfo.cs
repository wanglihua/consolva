using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单上下架排班
/// </summary>
public class ExpressShelfScheduleInfo : Entity
{
    /// <summary>
    /// 快递单，非空
    /// </summary>
    [Required(ErrorMessage = "快递单不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ();

    /// <summary>
    /// 上架人，非空
    /// </summary>
    [Required(ErrorMessage = "上架人不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 排班类型，非空
    /// </summary>
    [Required(ErrorMessage = "排班类型不能为空！")]
    [JsonProperty("scheduleType")]
    [ModelBinder(typeof(ExpressShelfScheduleTypeModelBinder))]
    public virtual ExpressShelfScheduleType ScheduleType { get; set; } = ExpressShelfScheduleType.None;

}