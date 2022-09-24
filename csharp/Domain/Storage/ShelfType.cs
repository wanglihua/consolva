using shunda.Core.CodeType;

namespace shunda.Domain.Storage;

public class ShelfType : CodeTypeBase
{
    /// <summary>
    /// 正常区, 1000
    /// </summary>
    public static readonly ShelfType Normal = new ShelfType(1000, "正常区", "正常区");

    /// <summary>
    /// 退货区, 2000
    /// </summary>
    public static readonly ShelfType Return = new ShelfType(2000, "退货区", "退货区");

    /// <summary>
    /// 打包区, 3000
    /// </summary>
    public static readonly ShelfType Pack = new ShelfType(3000, "打包区", "打包区");

    /// <summary>
    /// 长期无认领堆存区, 4000
    /// </summary>
    public static readonly ShelfType LongNoPickDump = new ShelfType(4000, "长期无认领堆存区", "长期无认领堆存区");

    /// <summary>
    /// 长期未出库堆存区, 5000
    /// </summary>
    public static readonly ShelfType LongNoProcessDump = new ShelfType(5000, "长期未出库堆存区", "长期未出库堆存区");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ShelfType None = new ShelfType(0, "", "");

    private static readonly List<ShelfType> codeTypeList = new List<ShelfType>() {
        Normal,
        Return,
        Pack,
        LongNoPickDump,
        LongNoProcessDump,
    };

    private static readonly Dictionary<int, ShelfType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ShelfType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ShelfType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ShelfType> GetList()
    {
        return codeTypeList;
    }

    private ShelfType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ShelfTypeHibernateType : CodeHibernateTypeBase<ShelfType>
{
    protected override ShelfType GetCodeTypeById(int id)
    {
        return ShelfType.GetById(id);
    }
}

public class ShelfTypeModelBinder : CodeTypeModelBinderBase<ShelfType>
{
    protected override ShelfType GetCodeTypeById(int id)
    {
        return ShelfType.GetById(id);
    }
}