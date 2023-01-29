using shunda.Domain.Fee;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

/// <summary>
/// 费用计算结果明细
/// </summary>
public class LogisticsFeeResultEntry {
    /// <summary>
    /// 费用名目
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 费用金额
    /// </summary>
    public decimal Fee { get; set; }

    /// <summary>
    /// 备注信息
    /// </summary>
    public decimal Note { get; set; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 费用类型
    /// </summary>
    public LogisticsOrderFeeType FeeType { get; set; } = LogisticsOrderFeeType.None;

    /// <summary>
    /// 结果明细类型
    /// </summary>
    public LogisticsFeeResultEntryType EntryType { get; set; }
}