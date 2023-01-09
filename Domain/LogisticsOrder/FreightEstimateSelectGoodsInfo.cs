using shunda.Core;

namespace shunda.Domain.LogisticsOrder;

public class FreightEstimateSelectGoodsInfo : EntityNoCreateNoUpdate {
    /// <summary>
    /// 客户ID，预估数据主键，非空
    /// </summary>
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 长度，非空
    /// </summary>
    public virtual decimal Length { get; set; }

    /// <summary>
    /// 宽度，非空
    /// </summary>
    public virtual decimal Width { get; set; }

    /// <summary>
    /// 高度，非空
    /// </summary>
    public virtual decimal Height { get; set; }

    /// <summary>
    /// 重量，非空
    /// </summary>
    public virtual decimal Weight { get; set; }

    /// <summary>
    /// 箱数，非空
    /// </summary>
    public virtual int PackageCount { get; set; }
}