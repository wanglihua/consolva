using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 体积重取向
/// </summary>
public class VolumeWeightOrientationType : CodeTypeBase
{
    /// <summary>
    /// 取大, 1000
    /// </summary>
    public static readonly VolumeWeightOrientationType Big = new VolumeWeightOrientationType(1000, "取大", "取大");

    /// <summary>
    /// 取中, 2000
    /// </summary>
    public static readonly VolumeWeightOrientationType Middle = new VolumeWeightOrientationType(2000, "取中", "取中");

    /// <summary>
    /// 取小, 3000
    /// </summary>
    public static readonly VolumeWeightOrientationType Small = new VolumeWeightOrientationType(3000, "取小", "取小");

    /// <summary>
    /// None
    /// </summary>
    private static readonly VolumeWeightOrientationType None = new VolumeWeightOrientationType(0, "", "");

    private static readonly IList<VolumeWeightOrientationType> codeTypeList = new List<VolumeWeightOrientationType>() {
        Big,
        Middle,
        Small,
    };

    private static readonly IDictionary<int, VolumeWeightOrientationType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static VolumeWeightOrientationType GetById(int id)
    {
        if (codeTypeMap.ContainsKey(id))
        {
            return codeTypeMap[id];
        }

        return None;
    }


    public static IList<VolumeWeightOrientationType> GetList()
    {
        return codeTypeList;
    }

    private VolumeWeightOrientationType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class VolumeWeightOrientationTypeHibernateType : CodeHibernateTypeBase<VolumeWeightOrientationType>
{
    protected override VolumeWeightOrientationType GetCodeTypeById(int id)
    {
        return VolumeWeightOrientationType.GetById(id);
    }
}

public class VolumeWeightOrientationTypeModelBinder : CodeTypeModelBinderBase<VolumeWeightOrientationType>
{
    protected override VolumeWeightOrientationType GetCodeTypeById(int id)
    {
        return VolumeWeightOrientationType.GetById(id);
    }
}