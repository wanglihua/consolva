using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackBoxHistoryType : CodeTypeBase {
    /// <summary>
    /// 打包
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType Pack = new LogisticsOrderPackBoxHistoryType(100, "打包", "打包");

    /// <summary>
    /// 转运
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType Transfer = new LogisticsOrderPackBoxHistoryType(350, "转运", "转运");

    /// <summary>
    /// 手工设置转运
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType TransferManual = new LogisticsOrderPackBoxHistoryType(355, "手工设置转运", "手工设置转运");

    /// <summary>
    /// 交付
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType Deliver = new LogisticsOrderPackBoxHistoryType(900, "交付", "交付");

    /// <summary>
    /// 手工设置交付
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType DeliverManual = new LogisticsOrderPackBoxHistoryType(1000, "手工设置交付", "手工设置交付");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderPackBoxHistoryType None = new LogisticsOrderPackBoxHistoryType(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderPackBoxHistoryType> codeTypeList = new List<LogisticsOrderPackBoxHistoryType>() {
        Pack,
        Transfer,
        TransferManual,
        Deliver,
        DeliverManual,
    };

    private static readonly IDictionary<int, LogisticsOrderPackBoxHistoryType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderPackBoxHistoryType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderPackBoxHistoryType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderPackBoxHistoryType> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsOrderPackBoxHistoryType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderPackBoxHistoryTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderPackBoxHistoryType> {
    protected override LogisticsOrderPackBoxHistoryType GetCodeTypeById(int id) {
        return LogisticsOrderPackBoxHistoryType.GetById(id);
    }
}

public class LogisticsOrderPackBoxHistoryTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderPackBoxHistoryType> {
    protected override LogisticsOrderPackBoxHistoryType GetCodeTypeById(int id) {
        return LogisticsOrderPackBoxHistoryType.GetById(id);
    }
}