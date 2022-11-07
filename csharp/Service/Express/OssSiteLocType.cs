using shunda.Core.CodeType;

namespace shunda.Service.Express;

public class OssSiteLocType : CodeTypeBase
{
    /// <summary>
    /// 香港
    /// </summary>
    public static readonly OssSiteLocType HongKong = new OssSiteLocType(100, "香港", "香港");

    /// <summary>
    /// 内地
    /// </summary>
    public static readonly OssSiteLocType China = new OssSiteLocType(200, "内地", "内地");

    /// <summary>
    /// None
    /// </summary>
    private static readonly OssSiteLocType None = new OssSiteLocType(0, "", "");

    private static readonly IReadOnlyList<OssSiteLocType> codeTypeList = new List<OssSiteLocType>() {
        HongKong,
        China,
    };

    private static readonly Dictionary<int, OssSiteLocType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static OssSiteLocType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out OssSiteLocType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<OssSiteLocType> GetList()
    {
        return codeTypeList.ToList();
    }

    private OssSiteLocType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class OssSiteLocTypeHibernateType : CodeHibernateTypeBase<OssSiteLocType>
{
    protected override OssSiteLocType GetCodeTypeById(int id)
    {
        return OssSiteLocType.GetById(id);
    }
}

public class OssSiteLocTypeModelBinder : CodeTypeModelBinderBase<OssSiteLocType>
{
    protected override OssSiteLocType GetCodeTypeById(int id)
    {
        return OssSiteLocType.GetById(id);
    }
}