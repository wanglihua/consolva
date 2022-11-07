using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单预告状态
/// </summary>
public class ExpressExpectStatus : CodeTypeBase
{
    /// <summary>
    /// 未到货
    /// </summary>
    public static readonly ExpressExpectStatus Create = new ExpressExpectStatus(1000, "未到货", "未到货");

    /// <summary>
    /// 可认领
    /// 录货值
    /// </summary>
    public static readonly ExpressExpectStatus Enter = new ExpressExpectStatus(2000, "录货值", "录货值");

    /// <summary>
    /// 已入库
    /// </summary>
    public static readonly ExpressExpectStatus Checkin = new ExpressExpectStatus(3000, "已入库", "已入库");


    /// <summary>
    /// None
    /// </summary>
    public static readonly ExpressExpectStatus None = new ExpressExpectStatus(0, "", "");

    public static ExpressExpectStatus GetById(int id)
    {
        IList<ExpressExpectStatus> typeList = GetList();
        foreach (ExpressExpectStatus type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<ExpressExpectStatus> GetList()
    {
        IList<ExpressExpectStatus> typeList = new List<ExpressExpectStatus>();

        typeList.Add(Create);
        typeList.Add(Enter);
        typeList.Add(Checkin);

        return typeList;
    }

    private ExpressExpectStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsExpressExpectStatusHibernateType : CodeHibernateTypeBase<ExpressExpectStatus>
{
    protected override ExpressExpectStatus GetCodeTypeById(int id)
    {
        return ExpressExpectStatus.GetById(id);
    }
}

public class LogisticsExpressExpectStatusModelBinder : CodeTypeModelBinderBase<ExpressExpectStatus>
{
    protected override ExpressExpectStatus GetCodeTypeById(int id)
    {
        return ExpressExpectStatus.GetById(id);
    }
}