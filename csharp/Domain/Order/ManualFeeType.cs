using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 人工操作费用类型
/// </summary>
public class ManualFeeType : CodeTypeBase
{
    /// <summary>
    /// 手动增加费用
    /// </summary>
    public static readonly ManualFeeType ManualIncrease = new ManualFeeType(1000, "手动增加", "手动增加费用");

    /// <summary>
    /// 手动减少费用
    /// </summary>
    public static readonly ManualFeeType ManualDecrease = new ManualFeeType(2000, "手动减少", "手动减少费用");

    /// <summary>
    /// 核销费用
    /// </summary>
    public static readonly ManualFeeType WriteOffFee = new ManualFeeType(3000, "核销费用", "核销费用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ManualFeeType None = new ManualFeeType(0, "None", "None");

    private static readonly List<ManualFeeType> codeTypeList = new List<ManualFeeType>() {
        ManualIncrease,
        ManualDecrease,
        WriteOffFee,
    };

    private static readonly Dictionary<int, ManualFeeType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ManualFeeType GetById(int typeId)
    {
        if (codeTypeMap.TryGetValue(typeId, out ManualFeeType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ManualFeeType> GetList()
    {
        return codeTypeList;
    }

    private ManualFeeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ManualFeeTypeHibernateType : CodeHibernateTypeBase<ManualFeeType>
{
    protected override ManualFeeType GetCodeTypeById(int id)
    {
        return ManualFeeType.GetById(id);
    }
}

public class ManualFeeTypeModelBinder : CodeTypeModelBinderBase<ManualFeeType>
{
    protected override ManualFeeType GetCodeTypeById(int id)
    {
        return ManualFeeType.GetById(id);
    }
}