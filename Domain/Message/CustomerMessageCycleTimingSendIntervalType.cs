using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息定时推送周期类型
/// </summary>
public class CustomerMessageCycleTimingSendIntervalType : CodeTypeBase
{
    /// <summary>
    ///  按周
    /// </summary>
    public static readonly CustomerMessageCycleTimingSendIntervalType Weeks = new CustomerMessageCycleTimingSendIntervalType(100, "按周", "按周");

    /// <summary>
    /// 按天
    /// </summary>
    public static readonly CustomerMessageCycleTimingSendIntervalType Days = new CustomerMessageCycleTimingSendIntervalType(200, "按天", "按天");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageCycleTimingSendIntervalType None = new CustomerMessageCycleTimingSendIntervalType(0, "", "");

    private static readonly IReadOnlyList<CustomerMessageCycleTimingSendIntervalType> codeTypeList = new List<CustomerMessageCycleTimingSendIntervalType>() {
        Weeks,
        Days,
    };

    private static readonly IDictionary<int, CustomerMessageCycleTimingSendIntervalType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageCycleTimingSendIntervalType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageCycleTimingSendIntervalType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerMessageCycleTimingSendIntervalType> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerMessageCycleTimingSendIntervalType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerMessageCycleTimingSendIntervalTypeHibernateType : CodeHibernateTypeBase<CustomerMessageCycleTimingSendIntervalType>
{
    protected override CustomerMessageCycleTimingSendIntervalType GetCodeTypeById(int id)
    {
        return CustomerMessageCycleTimingSendIntervalType.GetById(id);
    }
}

public class CustomerMessageCycleTimingSendIntervalTypeModelBinder : CodeTypeModelBinderBase<CustomerMessageCycleTimingSendIntervalType>
{
    protected override CustomerMessageCycleTimingSendIntervalType GetCodeTypeById(int id)
    {
        return CustomerMessageCycleTimingSendIntervalType.GetById(id);
    }
}