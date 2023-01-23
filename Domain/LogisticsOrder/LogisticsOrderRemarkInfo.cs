using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单备注
/// </summary>
public class LogisticsOrderRemarkInfo : Entity {
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 备注类型，非空
    /// </summary>
    [Required(ErrorMessage = "备注类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(LogisticsOrderRemarkTypeModelBinder))]
    public virtual LogisticsOrderRemarkType Type { get; set; } = LogisticsOrderRemarkType.None;

    /// <summary>
    /// 备注，非空
    /// </summary>
    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    [JsonProperty("remark")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 添加用户，可空
    /// </summary>
    [JsonProperty("addUser")]
    public virtual UserInfo? AddUser { get; set; } = null;
}