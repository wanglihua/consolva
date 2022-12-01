using shunda.Core.CodeType;

namespace shunda.Domain.Storage;

/// <summary>
/// 货架类别
/// </summary>
public class ShelfCate : CodeTypeBase
{
    /// <summary>
    /// 普通货架, 1000
    /// </summary>
    public static readonly ShelfCate Normal = new ShelfCate(1000, "普通货架", "普通货架");

    /// <summary>
    /// 地面仓位, 2000
    /// </summary>
    public static readonly ShelfCate Ground = new ShelfCate(2000, "地面仓位", "地面仓位");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ShelfCate None = new ShelfCate(0, "", "");

    private static readonly IReadOnlyList<ShelfCate> codeTypeList = new List<ShelfCate>() {
        Normal,
        Ground,
    };

    private static readonly Dictionary<int, ShelfCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ShelfCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ShelfCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ShelfCate> GetList()
    {
        return codeTypeList.ToList();
    }

    private ShelfCate(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ShelfCateHibernateType : CodeHibernateTypeBase<ShelfCate>
{
    protected override ShelfCate GetCodeTypeById(int id)
    {
        return ShelfCate.GetById(id);
    }
}

public class ShelfCateModelBinder : CodeTypeModelBinderBase<ShelfCate>
{
    protected override ShelfCate GetCodeTypeById(int id)
    {
        return ShelfCate.GetById(id);
    }
}