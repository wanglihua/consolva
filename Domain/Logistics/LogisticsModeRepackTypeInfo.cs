using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式打包方式
/// </summary>
public class LogisticsModeRepackTypeInfo : Entity
{
    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    [JsonProperty("logisticsMode")]
    public virtual LogisticsModeInfo LogisticsMode { get; set; } = new ();

    /// <summary>
    /// 打包方式，非空
    /// </summary>
    [Required(ErrorMessage = "打包方式不能为空！")]
    [JsonProperty("repackType")]
    [ModelBinder(typeof(LogisticsRepackTypeModelBinder))]
    public virtual LogisticsRepackType RepackType { get; set; } = LogisticsRepackType.None;
}