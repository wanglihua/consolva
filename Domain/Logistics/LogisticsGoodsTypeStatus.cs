using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsRepackTypeStatus : CodeTypeBase {
    /// <summary>
    /// 启用, 1000
    /// </summary>
    public static readonly LogisticsRepackTypeStatus Enabled = new LogisticsRepackTypeStatus(1000, "启用", "启用");

    /// <summary>
    /// 停用, 2000
    /// </summary>
    public static readonly LogisticsRepackTypeStatus Disabled = new LogisticsRepackTypeStatus(2000, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsRepackTypeStatus None = new LogisticsRepackTypeStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsRepackTypeStatus> codeTypeList = new List<LogisticsRepackTypeStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsRepackTypeStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsRepackTypeStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsRepackTypeStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsRepackTypeStatus> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsRepackTypeStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsRepackTypeStatusHibernateType : CodeHibernateTypeBase<LogisticsRepackTypeStatus> {
    protected override LogisticsRepackTypeStatus GetCodeTypeById(int id) {
        return LogisticsRepackTypeStatus.GetById(id);
    }
}

public class LogisticsRepackTypeStatusModelBinder : CodeTypeModelBinderBase<LogisticsRepackTypeStatus> {
    protected override LogisticsRepackTypeStatus GetCodeTypeById(int id) {
        return LogisticsRepackTypeStatus.GetById(id);
    }
}