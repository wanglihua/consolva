using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式渠道
/// </summary>
public class LogisticsModeChannel : CodeTypeBase
{
    /// <summary>
    /// 普货专线, 1000
    /// </summary>
    public static readonly LogisticsModeChannel Common = new LogisticsModeChannel(1000, "普货专线", "普货专线");

    /// <summary>
    /// 化妆品渠道, 2000
    /// </summary>
    public static readonly LogisticsModeChannel Cosmetics = new LogisticsModeChannel(2000, "化妆品渠道", "化妆品渠道");

    /// <summary>
    /// 化妆品渠道, 3000
    /// </summary>
    public static readonly LogisticsModeChannel Electric = new LogisticsModeChannel(3000, "化妆品渠道", "化妆品渠道");

    /// <summary>
    /// EMS渠道, 4000
    /// </summary>
    public static readonly LogisticsModeChannel Ems = new LogisticsModeChannel(4000, "EMS渠道", "EMS渠道");

    /// <summary>
    /// FedEx-联邦渠道, 5000
    /// </summary>
    public static readonly LogisticsModeChannel FedEx = new LogisticsModeChannel(5000, "FedEx-联邦渠道", "FedEx-联邦渠道");

    /// <summary>
    /// 其他, 100000
    /// </summary>
    public static readonly LogisticsModeChannel Other = new LogisticsModeChannel(100_000, "其他", "其他");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsModeChannel None = new LogisticsModeChannel(0, "", "");

    private static readonly IReadOnlyList<LogisticsModeChannel> codeTypeList = new List<LogisticsModeChannel>() {
        Common,
        Cosmetics,
        Electric,
        Ems,
        FedEx,
        Other,
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

    public static List<LogisticsModeChannel> GetList()
    {
        return codeTypeList.ToList();
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
