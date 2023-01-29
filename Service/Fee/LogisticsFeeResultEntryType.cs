namespace shunda.Service.Fee;

// todo 这里为什么没有 保险费用类型

/// <summary>
/// 费用计算结果明细类型
/// </summary>
public enum LogisticsFeeResultEntryType {
    /// <summary>
    /// 单笔费用
    /// </summary>
    Single,

    /// <summary>
    /// GST 费用
    /// </summary>
    Gst,

    /// <summary>
    /// 小计费用
    /// </summary>
    SubSum,

    /// <summary>
    /// 备注信息条目, 一般这时费用记录为零
    /// </summary>
    Note
}