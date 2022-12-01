using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsModeStatus : CodeTypeBase
{
    /// <summary>
    /// 启用
    /// </summary>
    public static readonly LogisticsModeStatus Enabled = new LogisticsModeStatus(1000, "启用", "启用");

    /// <summary>
    /// 禁用
    /// </summary>
    public static readonly LogisticsModeStatus Disabled = new LogisticsModeStatus(2000, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeStatus None = new LogisticsModeStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeStatus> codeTypeList = new List<LogisticsModeStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsModeStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    /// <summary>
    /// 获取状态列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsModeStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    /// <summary>
    /// 根据 Id 获取物流方式状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static LogisticsModeStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsModeStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    private LogisticsModeStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsModeStatusHibernateType : CodeHibernateTypeBase<LogisticsModeStatus>
{
    protected override LogisticsModeStatus GetCodeTypeById(int id)
    {
        return LogisticsModeStatus.GetById(id);
    }
}

public class LogisticsModeStatusModelBinder : CodeTypeModelBinderBase<LogisticsModeStatus>
{
    protected override LogisticsModeStatus GetCodeTypeById(int id)
    {
        return LogisticsModeStatus.GetById(id);
    }
}