using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单归档类型
/// </summary>
public class ExpressArchiveType : CodeTypeBase
{
    /// <summary>
    /// 待上架归档，1000
    /// </summary>
    public static readonly ExpressArchiveType ToOnShelf = new ExpressArchiveType(1000, "待上架归档", "待上架归档");

    /// <summary>
    /// 待下架归档，2000
    /// </summary>
    public static readonly ExpressArchiveType ToOffShelf = new ExpressArchiveType(2000, "待下架归档", "待下架归档");

    /// <summary>
    /// 长期未认领归档，3000
    /// </summary>
    public static readonly ExpressArchiveType NotPickedLong = new ExpressArchiveType(3000, "长期未认领归档", "长期未认领归档");

    /// <summary>
    /// 长期未出库归档，4000
    /// </summary>
    public static readonly ExpressArchiveType NotOrderLong = new ExpressArchiveType(4000, "长期未出库归档", "长期未出库归档");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressArchiveType None = new ExpressArchiveType(0, "", "");

    private static readonly IReadOnlyList<ExpressArchiveType> codeTypeList = new List<ExpressArchiveType>() {
        ToOnShelf,
        ToOffShelf,
        NotPickedLong,
        NotOrderLong,
    };

    private static readonly Dictionary<int, ExpressArchiveType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressArchiveType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressArchiveType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressArchiveType> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressArchiveType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressArchiveTypeHibernateType : CodeHibernateTypeBase<ExpressArchiveType>
{
    protected override ExpressArchiveType GetCodeTypeById(int id)
    {
        return ExpressArchiveType.GetById(id);
    }
}

public class ExpressArchiveTypeModelBinder : CodeTypeModelBinderBase<ExpressArchiveType>
{
    protected override ExpressArchiveType GetCodeTypeById(int id)
    {
        return ExpressArchiveType.GetById(id);
    }
}