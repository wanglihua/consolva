using Newtonsoft.Json;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 创建这个实体的目的是优化数据访问
/// </summary>
public class CustomerCheckinCodeInfo : EntityNoCreateNoUpdate {
    /// <summary>
    /// 入库校验，非空
    /// </summary>
    [JsonProperty("checkinCode")]
    public virtual string CheckinCode { get; set; } = string.Empty;
}