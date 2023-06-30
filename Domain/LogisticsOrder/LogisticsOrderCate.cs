using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单类别
/// </summary>
public class LogisticsOrderCate : CodeTypeBase {
    /// <summary>
    /// 空运
    /// </summary>
    public static readonly LogisticsOrderCate Air = new LogisticsOrderCate(1000, "空运", "空运");

    /// <summary>
    /// 海运
    /// </summary>
    public static readonly LogisticsOrderCate Sea = new LogisticsOrderCate(2000, "海运", "海运");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderCate None = new LogisticsOrderCate(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderCate> codeTypeList = new List<LogisticsOrderCate>() {
        Air,
        Sea,
    };

    private static readonly Dictionary<int, LogisticsOrderCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderCate GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderCate? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderCate> GetList() {
        return codeTypeList.ToList();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    private LogisticsOrderCate(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderCateHibernateType : CodeHibernateTypeBase<LogisticsOrderCate> {
    protected override LogisticsOrderCate GetCodeTypeById(int id) {
        return LogisticsOrderCate.GetById(id);
    }
}

public class LogisticsOrderCateModelBinder : CodeTypeModelBinderBase<LogisticsOrderCate> {
    protected override LogisticsOrderCate GetCodeTypeById(int id) {
        return LogisticsOrderCate.GetById(id);
    }
}