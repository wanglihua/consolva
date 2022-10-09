using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 订单计费方式
/// </summary>
public class LogisticsOrderFeeMethod : CodeTypeBase
{
    /// <summary>
    /// 打包箱号
    /// </summary>
    public static readonly LogisticsOrderFeeMethod Pack = new LogisticsOrderFeeMethod(100, "打包箱号", "打包箱号");

    /// <summary>
    /// 快递单号
    /// </summary>
    public static readonly LogisticsOrderFeeMethod Express = new LogisticsOrderFeeMethod(200, "快递单号", "快递单号");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderFeeMethod None = new LogisticsOrderFeeMethod(0, "", "");

    private static readonly List<LogisticsOrderFeeMethod> codeTypeList = new List<LogisticsOrderFeeMethod>() {
        Pack,
        Express,
    };

    private static readonly Dictionary<int, LogisticsOrderFeeMethod> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderFeeMethod GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderFeeMethod? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderFeeMethod> GetList()
    {
        return codeTypeList;
    }

    private LogisticsOrderFeeMethod(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderFeeMethodHibernateType : CodeHibernateTypeBase<LogisticsOrderFeeMethod>
{
    protected override LogisticsOrderFeeMethod GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeMethod.GetById(id);
    }
}

public class LogisticsOrderFeeMethodModelBinder : CodeTypeModelBinderBase<LogisticsOrderFeeMethod>
{
    protected override LogisticsOrderFeeMethod GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeMethod.GetById(id);
    }
}