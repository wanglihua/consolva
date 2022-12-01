using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单备注预设
/// </summary>
public class ExpressRemarkPresetSelectInfo : Entity
{
    /// <summary>
    /// 备注类型，非空
    /// </summary>
    [Required(ErrorMessage = "备注类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(ExpressRemarkTypeModelBinder))]
    public virtual ExpressRemarkType Type { get; set; } = ExpressRemarkType.None;

    /// <summary>
    /// 备注，非空
    /// </summary>
    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    [JsonProperty("remark")]
    public virtual string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 排序，非空
    /// </summary>
    [Required(ErrorMessage = "排序不能为空！")]
    [JsonProperty("orderNum")]
    public virtual int OrderNum { get; set; } = 0;

}