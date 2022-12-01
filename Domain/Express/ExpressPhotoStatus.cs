using shunda.Core.CodeType;

namespace shunda.Domain.Express;

public class ExpressPhotoStatus : CodeTypeBase
{
    /// <summary>
    /// 正常
    /// </summary>
    public static readonly ExpressPhotoStatus Normal = new ExpressPhotoStatus(1000, "正常", "正常");

    /// <summary>
    /// 照片无效
    /// 主要用于让某些照片不显示，比如说开始时拍的无效的入库照片
    /// </summary>
    public static readonly ExpressPhotoStatus Invalid = new ExpressPhotoStatus(99000, "照片无效", "照片无效");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressPhotoStatus None = new ExpressPhotoStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressPhotoStatus> codeTypeList = new List<ExpressPhotoStatus>() {
        Normal,
        Invalid,
    };

    private static readonly Dictionary<int, ExpressPhotoStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);
    
    public static ExpressPhotoStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressPhotoStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressPhotoStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressPhotoStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressPhotoStatusHibernateType : CodeHibernateTypeBase<ExpressPhotoStatus>
{
    protected override ExpressPhotoStatus GetCodeTypeById(int id)
    {
        return ExpressPhotoStatus.GetById(id);
    }
}

public class ExpressPhotoStatusModelBinder : CodeTypeModelBinderBase<ExpressPhotoStatus>
{
    protected override ExpressPhotoStatus GetCodeTypeById(int id)
    {
        return ExpressPhotoStatus.GetById(id);
    }
}