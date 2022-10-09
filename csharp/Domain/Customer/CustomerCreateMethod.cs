using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户创建方式
/// </summary>
public class CustomerCreateMethod : CodeTypeBase
{
    /// <summary>
    /// 前台注册, 1000
    /// </summary>
    public static readonly CustomerCreateMethod Register = new CustomerCreateMethod(1000, "前台注册", "前台注册");

    /// <summary>
    /// 后台导入, 2000
    /// </summary>
    public static readonly CustomerCreateMethod Import = new CustomerCreateMethod(2000, "后台导入", "后台导入");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerCreateMethod None = new CustomerCreateMethod(0, "", "");

    private static readonly List<CustomerCreateMethod> codeTypeList = new List<CustomerCreateMethod>() {
        Register,
        Import,
    };

    private static readonly Dictionary<int, CustomerCreateMethod> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerCreateMethod GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerCreateMethod? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static List<CustomerCreateMethod> GetList()
    {
        return codeTypeList;
    }

    private CustomerCreateMethod(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerCreateMethodHibernateType : CodeHibernateTypeBase<CustomerCreateMethod>
{
    protected override CustomerCreateMethod GetCodeTypeById(int id)
    {
        return CustomerCreateMethod.GetById(id);
    }
}

public class CustomerCreateMethodModelBinder : CodeTypeModelBinderBase<CustomerCreateMethod>
{
    protected override CustomerCreateMethod GetCodeTypeById(int id)
    {
        return CustomerCreateMethod.GetById(id);
    }
}
