using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式物品类型
/// </summary>
public partial class LogisticsModeGoodsTypeInfo : Entity
{
    /// <summary>
    /// 物流方式，非空
    /// </summary>
    public virtual LogisticsModeInfo LogisticsMode { get; set; } = new ();

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;
}