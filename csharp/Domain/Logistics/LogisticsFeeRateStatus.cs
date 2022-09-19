using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsFeeRateStatus : CodeTypeBase
{
    /// <summary>
    /// 启用
    /// </summary>
    public static readonly LogisticsFeeRateStatus Enabled = new LogisticsFeeRateStatus(100, "启用", "启用");

    /// <summary>
    /// 禁用
    /// </summary>
    public static readonly LogisticsFeeRateStatus Disabled = new LogisticsFeeRateStatus(200, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsFeeRateStatus None = new LogisticsFeeRateStatus(0, "", "");

    private static readonly List<LogisticsFeeRateStatus> codeTypeList = new List<LogisticsFeeRateStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsFeeRateStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    /// <summary>
    /// 获取状态列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsFeeRateStatus> GetList()
    {
        return codeTypeList;
    }

    /// <summary>
    /// 根据 Id 获取费率状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static LogisticsFeeRateStatus GetById(int id)
    {
        if (codeTypeMap.ContainsKey(id))
        {
            return codeTypeMap[id];
        }

        return None;
    }

    private LogisticsFeeRateStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsFeeRateStatusHibernateType : CodeHibernateTypeBase<LogisticsFeeRateStatus>
{
    protected override LogisticsFeeRateStatus GetCodeTypeById(int id)
    {
        return LogisticsFeeRateStatus.GetById(id);
    }
}

public class LogisticsFeeRateStatusModelBinder : CodeTypeModelBinderBase<LogisticsFeeRateStatus>
{
    protected override LogisticsFeeRateStatus GetCodeTypeById(int id)
    {
        return LogisticsFeeRateStatus.GetById(id);
    }
}