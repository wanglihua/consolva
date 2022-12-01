using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 货物用户类型
/// </summary>
public class ShelfUserType : CodeTypeBase
{
    /// <summary>
    /// 入库
    /// </summary>
    public static readonly ShelfUserType Checkin = new ShelfUserType(100, "入库", "入库");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ShelfUserType None = new ShelfUserType(0, "", "");

    private static readonly IReadOnlyList<ShelfUserType> codeTypeList = new List<ShelfUserType>() {
        Checkin,
    };

    private static readonly IDictionary<int, ShelfUserType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ShelfUserType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ShelfUserType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ShelfUserType> GetList()
    {
        return codeTypeList.ToList();
    }

    private ShelfUserType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ShelfUserTypeHibernateType : CodeHibernateTypeBase<ShelfUserType>
{
    protected override ShelfUserType GetCodeTypeById(int id)
    {
        return ShelfUserType.GetById(id);
    }
}

public class ShelfUserTypeModelBinder : CodeTypeModelBinderBase<ShelfUserType>
{
    protected override ShelfUserType GetCodeTypeById(int id)
    {
        return ShelfUserType.GetById(id);
    }
}