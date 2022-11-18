using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单创建方式
/// </summary>
public class LogisticsOrderCreateMethod : CodeTypeBase
{
    /// <summary>
    /// 客户创建, 1000
    /// </summary>
    public static readonly LogisticsOrderCreateMethod CustomerCreate = new LogisticsOrderCreateMethod(1000, "客户创建", "客户创建");

    /// <summary>
    /// 后台导入, 2000
    /// </summary>
    public static readonly LogisticsOrderCreateMethod UserImport = new LogisticsOrderCreateMethod(2000, "后台导入", "后台导入");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderCreateMethod None = new LogisticsOrderCreateMethod(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderCreateMethod> codeTypeList = new List<LogisticsOrderCreateMethod>() {
        CustomerCreate,
        UserImport,
    };

    private static readonly Dictionary<int, LogisticsOrderCreateMethod> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderCreateMethod GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderCreateMethod? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderCreateMethod> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsOrderCreateMethod(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderCreateMethodHibernateType : CodeHibernateTypeBase<LogisticsOrderCreateMethod>
{
    protected override LogisticsOrderCreateMethod GetCodeTypeById(int id)
    {
        return LogisticsOrderCreateMethod.GetById(id);
    }
}

public class LogisticsOrderCreateMethodModelBinder : CodeTypeModelBinderBase<LogisticsOrderCreateMethod>
{
    protected override LogisticsOrderCreateMethod GetCodeTypeById(int id)
    {
        return LogisticsOrderCreateMethod.GetById(id);
    }
}