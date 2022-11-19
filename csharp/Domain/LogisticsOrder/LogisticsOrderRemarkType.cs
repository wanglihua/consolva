using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单备注类型
/// </summary>
public class LogisticsOrderRemarkType : CodeTypeBase
{
    /// <summary>
    /// 客户备注
    /// </summary>
    public static readonly LogisticsOrderRemarkType Customer = new LogisticsOrderRemarkType(100, "客户备注", "客户备注");

    /// <summary>
    /// 后台备注
    /// </summary>
    public static readonly LogisticsOrderRemarkType Admin = new LogisticsOrderRemarkType(200, "后台备注", "后台备注");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderRemarkType None = new LogisticsOrderRemarkType(0, "", "");


    public static LogisticsOrderRemarkType GetById(int id)
    {
        IList<LogisticsOrderRemarkType> typeList = GetList();
        foreach (LogisticsOrderRemarkType orderTypeInfo in typeList)
        {
            if (orderTypeInfo.Id == id)
            {
                return orderTypeInfo;
            }
        }

        return None;
    }

    public static IList<LogisticsOrderRemarkType> GetList()
    {
        IList<LogisticsOrderRemarkType> typeList = new List<LogisticsOrderRemarkType>();
        typeList.Add(Customer);
        typeList.Add(Admin);

        return typeList;
    }

    private LogisticsOrderRemarkType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderRemarkTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderRemarkType>
{
    protected override LogisticsOrderRemarkType GetCodeTypeById(int id)
    {
        return LogisticsOrderRemarkType.GetById(id);
    }
}

public class LogisticsOrderRemarkTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderRemarkType>
{
    protected override LogisticsOrderRemarkType GetCodeTypeById(int id)
    {
        return LogisticsOrderRemarkType.GetById(id);
    }
}