using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单图片存储位置
/// </summary>
public class ExpressPhotoStoreLoc : CodeTypeBase
{
    /// <summary>
    /// Ecs服务器, 1000
    /// </summary>
    public static readonly ExpressPhotoStoreLoc Ecs = new ExpressPhotoStoreLoc(1000, "Ecs服务器", "Ecs服务器");

    /// <summary>
    /// 阿里云OSS, 2000
    /// </summary>
    public static readonly ExpressPhotoStoreLoc Oss = new ExpressPhotoStoreLoc(2000, "阿里云OSS", "阿里云OSS");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressPhotoStoreLoc None = new ExpressPhotoStoreLoc(0, "", "");

    private static readonly IReadOnlyList<ExpressPhotoStoreLoc> codeTypeList = new List<ExpressPhotoStoreLoc>() {
        Ecs,
        Oss,
    };

    private static readonly Dictionary<int, ExpressPhotoStoreLoc> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressPhotoStoreLoc GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressPhotoStoreLoc? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<ExpressPhotoStoreLoc> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressPhotoStoreLoc(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressPhotoStoreLocHibernateType : CodeHibernateTypeBase<ExpressPhotoStoreLoc>
{
    protected override ExpressPhotoStoreLoc GetCodeTypeById(int id)
    {
        return ExpressPhotoStoreLoc.GetById(id);
    }
}

public class ExpressPhotoStoreLocModelBinder : CodeTypeModelBinderBase<ExpressPhotoStoreLoc>
{
    protected override ExpressPhotoStoreLoc GetCodeTypeById(int id)
    {
        return ExpressPhotoStoreLoc.GetById(id);
    }
}
