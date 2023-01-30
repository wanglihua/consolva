namespace shunda.Service.Fee;

/// <summary>
/// 费用计算结果
/// </summary>
public class LogisticsFeeResult {
    /// <summary>
    /// 头程(含折扣)明细
    /// </summary>
    public IList<LogisticsFeeResultEntry> FirstModeFeeEntryList { get; set; } = new List<LogisticsFeeResultEntry>();

    /// <summary>
    /// 头程(考虑折扣)费用，FirstModeFeeEntryList中费用的总合
    /// </summary>
    public decimal FirstModeFee { get; set; }

    public decimal InsuranceFee { get; set; }

    public decimal GstFee { get; set; }

    /// <summary>
    /// 全部费用明细，包括 头程，保险，GST 
    /// </summary>
    public IList<LogisticsFeeResultEntry> FeeEntryList { get; set; } = new List<LogisticsFeeResultEntry>();

    /// <summary>
    /// 合计费用，FeeEntryList中费用的总合
    /// </summary>
    public decimal TotalFee { get; set; }
}