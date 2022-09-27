using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 体积重取向
/// </summary>
public class VolumeWeightResultType : CodeTypeBase
{
    /// <summary>
    /// 取大, 1000
    /// </summary>
    public static readonly VolumeWeightResultType Big = new VolumeWeightResultType(1000, "取大", "取大");

    /// <summary>
    /// 取中, 2000
    /// </summary>
    public static readonly VolumeWeightResultType Middle = new VolumeWeightResultType(2000, "取中", "取中");

    /// <summary>
    /// 取小, 3000
    /// </summary>
    public static readonly VolumeWeightResultType Small = new VolumeWeightResultType(3000, "取小", "取小");

    /// <summary>
    /// None
    /// </summary>
    private static readonly VolumeWeightResultType None = new VolumeWeightResultType(0, "", "");

    private static readonly IList<VolumeWeightResultType> codeTypeList = new List<VolumeWeightResultType>() {
        Big,
        Middle,
        Small,
    };

    private static readonly IDictionary<int, VolumeWeightResultType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static VolumeWeightResultType GetById(int id)
    {
        if (codeTypeMap.ContainsKey(id))
        {
            return codeTypeMap[id];
        }

        return None;
    }


    public static IList<VolumeWeightResultType> GetList()
    {
        return codeTypeList;
    }

    private VolumeWeightResultType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class VolumeWeightResultTypeHibernateType : CodeHibernateTypeBase<VolumeWeightResultType>
{
    protected override VolumeWeightResultType GetCodeTypeById(int id)
    {
        return VolumeWeightResultType.GetById(id);
    }
}

public class VolumeWeightResultTypeModelBinder : CodeTypeModelBinderBase<VolumeWeightResultType>
{
    protected override VolumeWeightResultType GetCodeTypeById(int id)
    {
        return VolumeWeightResultType.GetById(id);
    }
}