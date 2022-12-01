using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单货架状态
/// </summary>
public class ExpressShelfStatus : CodeTypeBase
{
    /// <summary>
    /// 已上架
    /// </summary>
    public static readonly ExpressShelfStatus ToOnShelf = new ExpressShelfStatus(50, "待上架", "待上架");

    /// <summary>
    /// 部分已上架
    /// </summary>
    public static readonly ExpressShelfStatus PartOnShelf = new ExpressShelfStatus(80, "部分已上架", "部分已上架");

    /// <summary>
    /// 已上架
    /// </summary>
    public static readonly ExpressShelfStatus OnShelf = new ExpressShelfStatus(100, "已上架", "已上架");

    /// <summary>
    /// 部分已下架
    /// </summary>
    public static readonly ExpressShelfStatus PartOffShelf = new ExpressShelfStatus(180, "部分已下架", "部分已下架");

    /// <summary>
    /// 已下架
    /// </summary>
    public static readonly ExpressShelfStatus OffShelf = new ExpressShelfStatus(200, "已下架", "已下架");

    /// <summary>
    /// 退运待下架
    /// </summary>
    public static readonly ExpressShelfStatus ReturnWaitOffShelf = new ExpressShelfStatus(300, "退运待下架", "退运待下架");

    /// <summary>
    /// 退运已下架
    /// </summary>
    public static readonly ExpressShelfStatus ReturnOffShelf = new ExpressShelfStatus(400, "退运已下架", "退运已下架");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressShelfStatus None = new ExpressShelfStatus(0, "", "");

    private static readonly IReadOnlyList<ExpressShelfStatus> codeTypeList = new List<ExpressShelfStatus>() {
        ToOnShelf,
        PartOnShelf,
        OnShelf,
        PartOffShelf,
        OffShelf,
        ReturnWaitOffShelf,
        ReturnOffShelf,
    };

    private static readonly Dictionary<int, ExpressShelfStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressShelfStatus GetById(int id)
    {
        if(codeTypeMap.TryGetValue(id, out ExpressShelfStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ExpressShelfStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressShelfStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressShelfStatusHibernateType : CodeHibernateTypeBase<ExpressShelfStatus>
{
    protected override ExpressShelfStatus GetCodeTypeById(int id)
    {
        return ExpressShelfStatus.GetById(id);
    }
}

public class ExpressShelfStatusModelBinder : CodeTypeModelBinderBase<ExpressShelfStatus>
{
    protected override ExpressShelfStatus GetCodeTypeById(int id)
    {
        return ExpressShelfStatus.GetById(id);
    }
}