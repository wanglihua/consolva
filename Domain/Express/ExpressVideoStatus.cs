using shunda.Core.CodeType;

namespace shunda.Domain.Express;

public class ExpressVideoStatus : CodeTypeBase
{
    /// <summary>
    /// 正常
    /// </summary>
    public static readonly ExpressVideoStatus Normal = new ExpressVideoStatus(100, "正常", "正常");

    /// <summary>
    /// 视频无效
    /// 主要用于让某些视频不显示，比如说开始时拍的无效的入库视频
    /// </summary>
    public static readonly ExpressVideoStatus Invalid = new ExpressVideoStatus(800_00, "视频无效", "视频无效");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressVideoStatus None = new ExpressVideoStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressVideoStatus> codeTypeList = new List<ExpressVideoStatus>() {
        Normal,
        Invalid,
    };

    private static readonly Dictionary<int, ExpressVideoStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressVideoStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressVideoStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressVideoStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressVideoStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressVideoStatusHibernateType : CodeHibernateTypeBase<ExpressVideoStatus>
{
    protected override ExpressVideoStatus GetCodeTypeById(int id)
    {
        return ExpressVideoStatus.GetById(id);
    }
}

public class ExpressVideoStatusModelBinder : CodeTypeModelBinderBase<ExpressVideoStatus>
{
    protected override ExpressVideoStatus GetCodeTypeById(int id)
    {
        return ExpressVideoStatus.GetById(id);
    }
}