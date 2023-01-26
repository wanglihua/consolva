using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackSortType : CodeTypeBase {
    /// <summary>
    /// 按照下单时间排序
    /// </summary>
    public static readonly LogisticsOrderPackSortType CreateTime = new LogisticsOrderPackSortType(1000, "按下单时间", "按下单时间");

    /// <summary>
    /// 按照提交打包时间排序
    /// </summary>
    public static readonly LogisticsOrderPackSortType SubmitPackTime = new LogisticsOrderPackSortType(2000, "按提交打包时间", "按提交打包时间");

    /// <summary>
    /// 按照仓位号排序
    /// </summary>
    public static readonly LogisticsOrderPackSortType StorageLoc = new LogisticsOrderPackSortType(3000, "按仓位号", "按仓位号");
    
    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderPackSortType None = new LogisticsOrderPackSortType(0, "", "");    

    private static readonly IReadOnlyList<LogisticsOrderPackSortType> codeTypeList = new List<LogisticsOrderPackSortType>() {
        CreateTime,
        SubmitPackTime,
        StorageLoc,
    };

    private static readonly IDictionary<int, LogisticsOrderPackSortType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderPackSortType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderPackSortType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderPackSortType> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsOrderPackSortType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderPackSortTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderPackSortType> {
    protected override LogisticsOrderPackSortType GetCodeTypeById(int id) {
        return LogisticsOrderPackSortType.GetById(id);
    }
}

public class LogisticsOrderPackSortTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderPackSortType> {
    protected override LogisticsOrderPackSortType GetCodeTypeById(int id) {
        return LogisticsOrderPackSortType.GetById(id);
    }
}
