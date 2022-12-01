using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using shunda.Core.Json;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单编码按客户
/// 使用了存储过程，这个类就基本上没用起来
/// </summary>
public class LogisticsOrderCodeCustomerInfo
{
    /// <summary>
    /// 客户Id，非空
    /// 这个就是主键
    /// </summary>
    [Required(ErrorMessage = "客户Id不能为空！")]
    [JsonProperty("customerId")]
    public virtual int CustomerId { get; set; } = 0;

    /// <summary>
    /// 序列号，非空
    /// </summary>
    [Required(ErrorMessage = "序列号不能为空！")]
    [JsonProperty("seq")]
    public virtual int Seq { get; set; } = 0;

    [BindNever()]
    [JsonProperty("createTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime CreateTime { get; set; } = DateTime.Now;

    [BindNever()]
    [JsonProperty("updateTime")]
    [JsonConverter(typeof(ShortDateTimeConverter))]
    public virtual DateTime UpdateTime { get; set; } = DateTime.Now;

}