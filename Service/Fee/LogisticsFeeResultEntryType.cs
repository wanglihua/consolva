namespace shunda.Service.Fee;

/// <summary>
/// 费用计算结果明细类型
/// </summary>
public enum LogisticsFeeResultEntryType {
    /// <summary>
    /// 单笔费用
    /// </summary>
    Single,

    /// <summary>
    /// 小计费用
    /// </summary>
    SubSum,

    /// <summary>
    /// 备注信息条目, 一般这时费用记录为零
    /// </summary>
    Note
}