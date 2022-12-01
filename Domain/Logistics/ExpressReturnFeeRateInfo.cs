using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 退运操作费率
/// </summary>
public partial class ExpressReturnFeeRateInfo : Entity
{
    /// <summary>
    /// 每快递操作费，非空
    /// </summary>
    public virtual decimal FeePerExpress { get; set; }
}