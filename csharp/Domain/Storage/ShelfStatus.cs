using shunda.Core.CodeType;

namespace shunda.Domain.Storage;

public class ShelfStatus : CodeTypeBase
{
    /// <summary>
    /// 启用, 1000
    /// </summary>
    public static readonly ShelfStatus Enabled = new ShelfStatus(1000, "启用", "启用");

    /// <summary>
    /// 停用, 2000
    /// </summary>
    public static readonly ShelfStatus Disabled = new ShelfStatus(2000, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ShelfStatus None = new ShelfStatus(0, "", "");

    private static readonly IReadOnlyList<ShelfStatus> codeTypeList = new List<ShelfStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, ShelfStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ShelfStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ShelfStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ShelfStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ShelfStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ShelfStatusHibernateType : CodeHibernateTypeBase<ShelfStatus>
{
    protected override ShelfStatus GetCodeTypeById(int id)
    {
        return ShelfStatus.GetById(id);
    }
}

public class ShelfStatusModelBinder : CodeTypeModelBinderBase<ShelfStatus>
{
    protected override ShelfStatus GetCodeTypeById(int id)
    {
        return ShelfStatus.GetById(id);
    }
}