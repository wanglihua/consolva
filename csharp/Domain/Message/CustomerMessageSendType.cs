using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息推送类型
/// </summary>
public class CustomerMessageSendType : CodeTypeBase
{
    /// <summary>
    /// 手动推送
    /// </summary>
    public static readonly CustomerMessageSendType Manual = new CustomerMessageSendType(1000, "手动推送", "手动推送方式");

    /// <summary>
    /// 定时推送
    /// </summary>
    public static readonly CustomerMessageSendType CycleTiming = new CustomerMessageSendType(2000, "定时推送", "定时推送方式");

    /// <summary>
    /// 实时推送
    /// </summary>
    public static readonly CustomerMessageSendType RealTime = new CustomerMessageSendType(3000, "实时推送", "实时推送方式");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageSendType None = new CustomerMessageSendType(0, "", "");

    private static readonly List<CustomerMessageSendType> codeTypeList = new List<CustomerMessageSendType>() {
        Manual,
        CycleTiming,
        RealTime,
    };

    private static readonly Dictionary<int, CustomerMessageSendType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageSendType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageSendType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerMessageSendType> GetList()
    {
        return codeTypeList;
    }

    private CustomerMessageSendType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerMessageSendTypeHibernateType : CodeHibernateTypeBase<CustomerMessageSendType>
{
    protected override CustomerMessageSendType GetCodeTypeById(int id)
    {
        return CustomerMessageSendType.GetById(id);
    }
}

public class CustomerMessageSendTypeModelBinder : CodeTypeModelBinderBase<CustomerMessageSendType>
{
    protected override CustomerMessageSendType GetCodeTypeById(int id)
    {
        return CustomerMessageSendType.GetById(id);
    }
}