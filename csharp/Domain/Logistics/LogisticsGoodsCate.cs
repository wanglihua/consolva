using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品类别
/// 暂时还没用起来，保留
/// </summary>
public class LogisticsGoodsCate : CodeTypeBase
{
    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsCate None = new LogisticsGoodsCate(0, "", "");

    private static readonly IReadOnlyList<LogisticsGoodsCate> codeTypeList = new List<LogisticsGoodsCate>() { };

    private static readonly Dictionary<int, LogisticsGoodsCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsGoodsCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsGoodsCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsGoodsCate> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsGoodsCate(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsGoodsCateHibernateType : CodeHibernateTypeBase<LogisticsGoodsCate>
{
    protected override LogisticsGoodsCate GetCodeTypeById(int id)
    {
        return LogisticsGoodsCate.GetById(id);
    }
}

public class LogisticsGoodsCateModelBinder : CodeTypeModelBinderBase<LogisticsGoodsCate>
{
    protected override LogisticsGoodsCate GetCodeTypeById(int id)
    {
        return LogisticsGoodsCate.GetById(id);
    }
}