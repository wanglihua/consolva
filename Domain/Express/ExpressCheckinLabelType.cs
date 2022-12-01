using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 入库标签打印类型
/// </summary>
public class ExpressCheckinLabelType : CodeTypeBase
{
    /// <summary>
    /// 10cm × 5cm 标签
    /// </summary>
    public static readonly ExpressCheckinLabelType SizeW10H05 = new ExpressCheckinLabelType(1000, "10cm × 5cm 标签", "10cm × 5cm 标签");

    /// <summary>
    /// 5cm × 5cm 标签
    /// </summary>
    public static readonly ExpressCheckinLabelType SizeW05H05 = new ExpressCheckinLabelType(2000, "5cm × 5cm 标签", "5cm × 5cm 标签");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressCheckinLabelType None = new ExpressCheckinLabelType(0, "", "");

    private static readonly IReadOnlyList<ExpressCheckinLabelType> codeTypeList = new List<ExpressCheckinLabelType>() {
        SizeW10H05,
        SizeW05H05,
    };

    private static readonly Dictionary<int, ExpressCheckinLabelType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressCheckinLabelType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressCheckinLabelType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressCheckinLabelType> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressCheckinLabelType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressCheckinPrintTypeHibernateType : CodeHibernateTypeBase<ExpressCheckinLabelType>
{
    protected override ExpressCheckinLabelType GetCodeTypeById(int id)
    {
        return ExpressCheckinLabelType.GetById(id);
    }
}

public class ExpressCheckinPrintTypeModelBinder : CodeTypeModelBinderBase<ExpressCheckinLabelType>
{
    protected override ExpressCheckinLabelType GetCodeTypeById(int id)
    {
        return ExpressCheckinLabelType.GetById(id);
    }
}