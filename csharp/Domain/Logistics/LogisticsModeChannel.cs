using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式渠道
/// </summary>
public class LogisticsModeChannel : CodeTypeBase
{
    /// <summary>
    /// EMS, 1000
    /// </summary>
    public static readonly LogisticsModeChannel Ems = new LogisticsModeChannel(1000, "EMS", "EMS");

    /// <summary>
    /// 其他, 100_000
    /// </summary>
    public static readonly LogisticsModeChannel Inner = new LogisticsModeChannel(100_000, "其他", "其他");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeChannel None = new LogisticsModeChannel(0, "", "");

    private static readonly List<LogisticsModeChannel> codeTypeList = new List<LogisticsModeChannel>() {
        Ems,
        Inner,
    };

    private static readonly Dictionary<int, LogisticsModeChannel> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsModeChannel GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsModeChannel? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsModeChannel> GetList()
    {
        return codeTypeList;
    }

    private LogisticsModeChannel(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsModeChannelHibernateType : CodeHibernateTypeBase<LogisticsModeChannel>
{
    protected override LogisticsModeChannel GetCodeTypeById(int id)
    {
        return LogisticsModeChannel.GetById(id);
    }
}

public class LogisticsModeChannelModelBinder : CodeTypeModelBinderBase<LogisticsModeChannel>
{
    protected override LogisticsModeChannel GetCodeTypeById(int id)
    {
        return LogisticsModeChannel.GetById(id);
    }
}