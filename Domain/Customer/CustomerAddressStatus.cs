using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户地址状态
/// </summary>
public class CustomerAddressStatus : CodeTypeBase
{
    /// <summary>
    /// 新创建, 1000
    /// </summary>
    public static readonly CustomerAddressStatus New = new CustomerAddressStatus(1000, "新创建", "新创建");

    /// <summary>
    /// 已验证, 2000
    /// </summary>
    public static readonly CustomerAddressStatus Valid = new CustomerAddressStatus(2000, "已验证", "已验证");

    /// <summary>
    /// 已失效, 3000
    /// </summary>
    public static readonly CustomerAddressStatus Invalid = new CustomerAddressStatus(3000, "已失效", "已失效");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerAddressStatus None = new CustomerAddressStatus(0, "", "");

    private static readonly IReadOnlyList<CustomerAddressStatus> codeTypeList = new List<CustomerAddressStatus>() {
        New,
        Valid,
        Invalid,
    };

    private static readonly Dictionary<int, CustomerAddressStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerAddressStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerAddressStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<CustomerAddressStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerAddressStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerAddressStatusHibernateType : CodeHibernateTypeBase<CustomerAddressStatus>
{
    protected override CustomerAddressStatus GetCodeTypeById(int id)
    {
        return CustomerAddressStatus.GetById(id);
    }
}

public class CustomerAddressStatusModelBinder : CodeTypeModelBinderBase<CustomerAddressStatus>
{
    protected override CustomerAddressStatus GetCodeTypeById(int id)
    {
        return CustomerAddressStatus.GetById(id);
    }
}
