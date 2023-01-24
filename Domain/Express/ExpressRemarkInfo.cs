using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单备注
/// </summary>
public class ExpressRemarkInfo : Entity {
    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new();

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
    [JsonProperty("content")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 添加用户，可空
    /// </summary>
    [JsonProperty("addUser")]
    public virtual UserInfo? AddUser { get; set; } = null;
}