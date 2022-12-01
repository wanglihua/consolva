using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递堆存状态
/// </summary>
public class ExpressDumpStatus : CodeTypeBase
{
    /// <summary>
    /// 未堆存，1000
    /// </summary>
    public static readonly ExpressDumpStatus NotDump = new ExpressDumpStatus(1000, "未堆存", "未堆存");

    /// <summary>
    /// 部分堆存，2000
    /// </summary>
    public static readonly ExpressDumpStatus PartDump = new ExpressDumpStatus(2000, "部分堆存", "部分堆存");

    /// <summary>
    /// 已堆存，3000
    /// </summary>
    public static readonly ExpressDumpStatus Dumped = new ExpressDumpStatus(3000, "已堆存", "已堆存");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressDumpStatus None = new ExpressDumpStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressDumpStatus> codeTypeList = new List<ExpressDumpStatus>() {
        NotDump,
        PartDump,
        Dumped,
    };

    private static readonly Dictionary<int, ExpressDumpStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressDumpStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressDumpStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressDumpStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressDumpStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressDumpStatusHibernateType : CodeHibernateTypeBase<ExpressDumpStatus>
{
    protected override ExpressDumpStatus GetCodeTypeById(int id)
    {
        return ExpressDumpStatus.GetById(id);
    }
}

public class ExpressDumpStatusModelBinder : CodeTypeModelBinderBase<ExpressDumpStatus>
{
    protected override ExpressDumpStatus GetCodeTypeById(int id)
    {
        return ExpressDumpStatus.GetById(id);
    }
}