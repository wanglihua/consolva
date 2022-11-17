using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品操作日志
/// </summary>
public class LogisticsGoodsLogInfo : EntityNoUpdate
{
    /// <summary>
    /// 操作类型，非空
    /// </summary>
    [Required(ErrorMessage = "操作类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(LogisticsGoodsLogTypeModelBinder))]
    public virtual LogisticsGoodsLogType Type { get; set; } = LogisticsGoodsLogType.None;

    /// <summary>
    /// 物品，非空
    /// </summary>
    [Required(ErrorMessage = "物品不能为空！")]
    [JsonProperty("goods")]
    public virtual LogisticsGoodsInfo Goods { get; set; } = new LogisticsGoodsInfo();

    /// <summary>
    /// 操作人，非空
    /// </summary>
    [Required(ErrorMessage = "操作人不能为空！")]
    [JsonProperty("user")]
    public virtual UserInfo User { get; set; } = new UserInfo();

    /// <summary>
    /// 操作备注，非空
    /// </summary>
    [Required(ErrorMessage = "操作备注不能为空！")]
    [MaxLength(800, ErrorMessage = "操作备注长度不能大于800！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

}