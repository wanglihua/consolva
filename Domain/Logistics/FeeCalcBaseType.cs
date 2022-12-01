using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 费用计算根据
///
/// 主要用于 保险和GST的费用计算基数
/// </summary>
public class FeeCalcBaseType : CodeTypeBase
{
    /// <summary>
    /// 运费总额, 1000
    /// </summary>
    public static readonly FeeCalcBaseType Fee = new FeeCalcBaseType(1000, "运费总额", "运费总额");

    /// <summary>
    /// 申报总额, 2000
    /// </summary>
    public static readonly FeeCalcBaseType Amount = new FeeCalcBaseType(2000, "申报总额", "申报总额");

    /// <summary>
    /// 运费+申报, 3000
    /// </summary>
    public static readonly FeeCalcBaseType FeeAndAmount = new FeeCalcBaseType(3000, "运费+申报", "运费+申报");

    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeCalcBaseType None = new FeeCalcBaseType(0, "", "");

    private static readonly IReadOnlyList<FeeCalcBaseType> codeTypeList = new List<FeeCalcBaseType>() {
        Fee,
        Amount,
        FeeAndAmount,
    };

    private static readonly Dictionary<int, FeeCalcBaseType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static FeeCalcBaseType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out FeeCalcBaseType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<FeeCalcBaseType> GetList()
    {
        return codeTypeList.ToList();
    }

    private FeeCalcBaseType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeCalcBaseTypeHibernateType : CodeHibernateTypeBase<FeeCalcBaseType>
{
    protected override FeeCalcBaseType GetCodeTypeById(int id)
    {
        return FeeCalcBaseType.GetById(id);
    }
}

public class FeeCalcBaseTypeModelBinder : CodeTypeModelBinderBase<FeeCalcBaseType>
{
    protected override FeeCalcBaseType GetCodeTypeById(int id)
    {
        return FeeCalcBaseType.GetById(id);
    }
}