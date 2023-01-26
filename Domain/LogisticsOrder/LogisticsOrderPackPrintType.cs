using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 打包标签打印类型
/// </summary>
public class LogisticsOrderPackPrintType : CodeTypeBase {
    /// <summary>
    /// 10cm × 15cm 标签
    /// </summary>
    public static readonly LogisticsOrderPackPrintType SizeW10H15 = new LogisticsOrderPackPrintType(1000, "10cm × 15cm 标签", "10cm × 15cm 标签");

    /// <summary>
    /// 10cm × 5cm 标签
    /// </summary>
    public static readonly LogisticsOrderPackPrintType SizeW10H5 = new LogisticsOrderPackPrintType(2000, "10cm × 5cm 标签", "10cm × 5cm 标签");

    /// <summary>
    /// 5cm × 5cm 标签
    /// </summary>
    public static readonly LogisticsOrderPackPrintType SizeW5H5 = new LogisticsOrderPackPrintType(3000, "5cm × 5cm 标签", "5cm × 5cm 标签");

    /// <summary>
    /// None
    /// 如果物流方式设置的是这个的值，就表示不打印打包标签
    /// </summary>
    public static readonly LogisticsOrderPackPrintType None = new LogisticsOrderPackPrintType(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderPackPrintType> codeTypeList = new List<LogisticsOrderPackPrintType>() {
        SizeW10H15,
        SizeW10H5,
        SizeW5H5,
    };

    private static readonly IDictionary<int, LogisticsOrderPackPrintType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);


    public static LogisticsOrderPackPrintType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderPackPrintType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderPackPrintType> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsOrderPackPrintType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderPackPrintTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderPackPrintType> {
    protected override LogisticsOrderPackPrintType GetCodeTypeById(int id) {
        return LogisticsOrderPackPrintType.GetById(id);
    }
}

public class LogisticsOrderPackPrintTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderPackPrintType> {
    protected override LogisticsOrderPackPrintType GetCodeTypeById(int id) {
        return LogisticsOrderPackPrintType.GetById(id);
    }
}