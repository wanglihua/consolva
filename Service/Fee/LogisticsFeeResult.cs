namespace shunda.Service.Fee;

/// <summary>
/// 费用计算结果
/// </summary>
public class LogisticsFeeResult {
    /// <summary>
    /// 头程(含折扣)明细
    /// </summary>
    public IList<LogisticsFeeResultEntry> FirstModeEntryList { get; set; } = new List<LogisticsFeeResultEntry>();

    /// <summary>
    /// 头程(考虑折扣)费用
    /// </summary>
    public decimal FirstModeFee { get; set; }

    public decimal InsuranceFee { get; set; }

    public decimal GstFee { get; set; }

    public IList<LogisticsFeeResultEntry> EntryList { get; set; } = new List<LogisticsFeeResultEntry>();

    /// <summary>
    /// 合计费用
    /// </summary>
    public decimal TotalFee { get; set; }
}