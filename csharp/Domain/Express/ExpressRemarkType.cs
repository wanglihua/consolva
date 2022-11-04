using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单备类型
/// </summary>
public class ExpressRemarkType : CodeTypeBase
{
    /// <summary>
    /// 入库备注, 1000
    /// </summary>
    public static readonly ExpressRemarkType Checkin = new ExpressRemarkType(1000, "入库备注", "入库备注");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressRemarkType None = new ExpressRemarkType(0, "", "");

    private static readonly List<ExpressRemarkType> codeTypeList = new List<ExpressRemarkType>() {
        Checkin,
    };

    private static readonly Dictionary<int, ExpressRemarkType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressRemarkType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressRemarkType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<ExpressRemarkType> GetList()
    {
        return codeTypeList;
    }

    private ExpressRemarkType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressRemarkTypeHibernateType : CodeHibernateTypeBase<ExpressRemarkType>
{
    protected override ExpressRemarkType GetCodeTypeById(int id)
    {
        return ExpressRemarkType.GetById(id);
    }
}

public class ExpressRemarkTypeModelBinder : CodeTypeModelBinderBase<ExpressRemarkType>
{
    protected override ExpressRemarkType GetCodeTypeById(int id)
    {
        return ExpressRemarkType.GetById(id);
    }
}
