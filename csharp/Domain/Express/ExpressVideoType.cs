using shunda.Core.CodeType;

namespace shunda.Domain.Express;

public class ExpressVideoType : CodeTypeBase
{
    /// <summary>
    /// 入库视频
    /// </summary>
    public static readonly ExpressVideoType CheckIn = new ExpressVideoType(100, "入库视频", "入库视频");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressVideoType None = new ExpressVideoType(0, "", "");

    private static readonly IReadOnlyList<ExpressVideoType> codeTypeList = new List<ExpressVideoType>() {
        CheckIn,
    };

    private static readonly Dictionary<int, ExpressVideoType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ExpressVideoType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ExpressVideoType? codeType))
        {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 返回所有支付类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<ExpressVideoType> GetList()
    {
        return codeTypeList.ToList();
    }

    private ExpressVideoType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ExpressVideoTypeHibernateType : CodeHibernateTypeBase<ExpressVideoType>
{
    protected override ExpressVideoType GetCodeTypeById(int id)
    {
        return ExpressVideoType.GetById(id);
    }
}

public class ExpressVideoTypeModelBinder : CodeTypeModelBinderBase<ExpressVideoType>
{
    protected override ExpressVideoType GetCodeTypeById(int id)
    {
        return ExpressVideoType.GetById(id);
    }
}