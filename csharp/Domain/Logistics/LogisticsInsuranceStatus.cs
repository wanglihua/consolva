using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 保险状态
/// </summary>
public class LogisticsInsuranceStatus : CodeTypeBase
{
    /// <summary>
    /// 启用, 1000
    /// </summary>
    public static readonly LogisticsInsuranceStatus Enabled = new LogisticsInsuranceStatus(1000, "启用", "启用");

    /// <summary>
    /// 禁用, 2000
    /// </summary>
    public static readonly LogisticsInsuranceStatus Disabled = new LogisticsInsuranceStatus(2000, "禁用", "禁用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsInsuranceStatus None = new LogisticsInsuranceStatus(0, "", "");

    private static readonly List<LogisticsInsuranceStatus> codeTypeList = new List<LogisticsInsuranceStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsInsuranceStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsInsuranceStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsInsuranceStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsInsuranceStatus> GetList()
    {
        return codeTypeList;
    }

    private LogisticsInsuranceStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsInsuranceStatusHibernateType : CodeHibernateTypeBase<LogisticsInsuranceStatus>
{
    protected override LogisticsInsuranceStatus GetCodeTypeById(int id)
    {
        return LogisticsInsuranceStatus.GetById(id);
    }
}

public class LogisticsInsuranceStatusModelBinder : CodeTypeModelBinderBase<LogisticsInsuranceStatus>
{
    protected override LogisticsInsuranceStatus GetCodeTypeById(int id)
    {
        return LogisticsInsuranceStatus.GetById(id);
    }
}