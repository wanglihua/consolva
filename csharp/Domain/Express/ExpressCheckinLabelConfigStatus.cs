using shunda.Core.CodeType;

namespace shunda.Domain.Express;

public class ExpressCheckinLabelConfigStatus : CodeTypeBase
{
    /// <summary>
    /// 未启用， 1000
    /// </summary>
    public static readonly ExpressCheckinLabelConfigStatus Disabled = new ExpressCheckinLabelConfigStatus(1000, "未启用", "未启用");

    /// <summary>
    /// 已启用， 2000
    /// </summary>
    public static readonly ExpressCheckinLabelConfigStatus Enabled = new ExpressCheckinLabelConfigStatus(2000, "已启用", "已启用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressCheckinLabelConfigStatus None = new ExpressCheckinLabelConfigStatus(0, "None", "None");

    private static readonly IReadOnlyList<ExpressCheckinLabelConfigStatus> codeTypeList = new List<ExpressCheckinLabelConfigStatus>() {
        Disabled,
        Enabled,
    };

    private static readonly Dictionary<int, ExpressCheckinLabelConfigStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressCheckinLabelConfigStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressCheckinLabelConfigStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressCheckinLabelConfigStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressCheckinLabelConfigStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressCheckinLabelConfigStatusHibernateType : CodeHibernateTypeBase<ExpressCheckinLabelConfigStatus>
{
    protected override ExpressCheckinLabelConfigStatus GetCodeTypeById(int id)
    {
        return ExpressCheckinLabelConfigStatus.GetById(id);
    }
}

public class ExpressCheckinLabelConfigStatusModelBinder : CodeTypeModelBinderBase<ExpressCheckinLabelConfigStatus>
{
    protected override ExpressCheckinLabelConfigStatus GetCodeTypeById(int id)
    {
        return ExpressCheckinLabelConfigStatus.GetById(id);
    }
}