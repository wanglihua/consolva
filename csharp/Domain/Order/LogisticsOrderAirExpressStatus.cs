using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 空运快递状态
/// </summary>
public class LogisticsOrderAirExpressStatus : CodeTypeBase
{
    /// <summary>
    /// 待发
    /// </summary>
    public static readonly LogisticsOrderAirExpressStatus ToSend = new LogisticsOrderAirExpressStatus(1000, "待发", "待发");

    /// <summary>
    /// 在途
    /// </summary>
    public static readonly LogisticsOrderAirExpressStatus Transiting = new LogisticsOrderAirExpressStatus(2000, "在途", "在途");

    /// <summary>
    /// 退运
    /// </summary>
    public static readonly LogisticsOrderAirExpressStatus Returned = new LogisticsOrderAirExpressStatus(3000, "退运", "退运");

    /// <summary>
    /// 妥投
    /// </summary>
    public static readonly LogisticsOrderAirExpressStatus Delivered = new LogisticsOrderAirExpressStatus(4000, "妥投", "妥投");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderAirExpressStatus None = new LogisticsOrderAirExpressStatus(0, "", "");

    private static readonly List<LogisticsOrderAirExpressStatus> codeTypeList = new List<LogisticsOrderAirExpressStatus>() {
        ToSend,
        Transiting,
        Returned,
        Delivered,
    };

    private static readonly Dictionary<int, LogisticsOrderAirExpressStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderAirExpressStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderAirExpressStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderAirExpressStatus> GetList()
    {
        return codeTypeList;
    }

    private LogisticsOrderAirExpressStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderAirExpressStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderAirExpressStatus>
{
    protected override LogisticsOrderAirExpressStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderAirExpressStatus.GetById(id);
    }
}

public class LogisticsOrderAirExpressStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderAirExpressStatus>
{
    protected override LogisticsOrderAirExpressStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderAirExpressStatus.GetById(id);
    }
}