using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递排班类型
/// </summary>
public class ExpressShelfScheduleType : CodeTypeBase
{
    /// <summary>
    /// 上架排班
    /// </summary>
    public static readonly ExpressShelfScheduleType OnShelf = new ExpressShelfScheduleType(100, "上架排班", "上架排班");

    /// <summary>
    /// 下架排班
    /// </summary>
    public static readonly ExpressShelfScheduleType OffShelf = new ExpressShelfScheduleType(200, "下架排班", "下架排班");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressShelfScheduleType None = new ExpressShelfScheduleType(0, "", "");

    private static readonly IReadOnlyList<ExpressShelfScheduleType> codeTypeList = new List<ExpressShelfScheduleType>() {
        OnShelf,
        OffShelf,
    };

    private static readonly Dictionary<int, ExpressShelfScheduleType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressShelfScheduleType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressShelfScheduleType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressShelfScheduleType> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressShelfScheduleType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressShelfScheduleTypeHibernateType : CodeHibernateTypeBase<ExpressShelfScheduleType>
{
    protected override ExpressShelfScheduleType GetCodeTypeById(int id)
    {
        return ExpressShelfScheduleType.GetById(id);
    }
}

public class ExpressShelfScheduleTypeModelBinder : CodeTypeModelBinderBase<ExpressShelfScheduleType>
{
    protected override ExpressShelfScheduleType GetCodeTypeById(int id)
    {
        return ExpressShelfScheduleType.GetById(id);
    }
}