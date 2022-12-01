using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递转储类型
/// </summary>
public class ExpressDumpType : CodeTypeBase
{
    /// <summary>
    /// 长期无认领
    /// </summary>
    public static readonly ExpressDumpType NoPickup = new ExpressDumpType(100, "长期无认领", "长期无认领");


    /// <summary>
    /// 长期未处理
    /// </summary>
    public static readonly ExpressDumpType NoProcess = new ExpressDumpType(200, "长期未处理", "长期未处理");


    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressDumpType None = new ExpressDumpType(0, "", "");

    public static ExpressDumpType GetById(int id)
    {
        IList<ExpressDumpType> typeList = GetList();
        foreach (ExpressDumpType type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<ExpressDumpType> GetList()
    {
        IList<ExpressDumpType> typeList = new List<ExpressDumpType>();

        typeList.Add(NoPickup);
        typeList.Add(NoProcess);

        return typeList;
    }

    private ExpressDumpType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressDumpTypeHibernateType : CodeHibernateTypeBase<ExpressDumpType>
{
    protected override ExpressDumpType GetCodeTypeById(int id)
    {
        return ExpressDumpType.GetById(id);
    }
}

public class ExpressDumpTypeModelBinder : CodeTypeModelBinderBase<ExpressDumpType>
{
    protected override ExpressDumpType GetCodeTypeById(int id)
    {
        return ExpressDumpType.GetById(id);
    }
}