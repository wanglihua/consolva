using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsTypeStatus : CodeTypeBase
{
    /// <summary>
    /// 启用, 1000
    /// </summary>
    public static readonly LogisticsGoodsTypeStatus Enabled = new LogisticsGoodsTypeStatus(1000, "启用", "启用");

    /// <summary>
    /// 停用, 2000
    /// </summary>
    public static readonly LogisticsGoodsTypeStatus Disabled = new LogisticsGoodsTypeStatus(2000, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsTypeStatus None = new LogisticsGoodsTypeStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsGoodsTypeStatus> codeTypeList = new List<LogisticsGoodsTypeStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, LogisticsGoodsTypeStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsGoodsTypeStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsGoodsTypeStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsGoodsTypeStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsGoodsTypeStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsGoodsTypeStatusHibernateType : CodeHibernateTypeBase<LogisticsGoodsTypeStatus>
{
    protected override LogisticsGoodsTypeStatus GetCodeTypeById(int id)
    {
        return LogisticsGoodsTypeStatus.GetById(id);
    }
}

public class LogisticsGoodsTypeStatusModelBinder : CodeTypeModelBinderBase<LogisticsGoodsTypeStatus>
{
    protected override LogisticsGoodsTypeStatus GetCodeTypeById(int id)
    {
        return LogisticsGoodsTypeStatus.GetById(id);
    }
}