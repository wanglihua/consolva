using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户状态
/// </summary>
public class CustomerStatus : CodeTypeBase
{
    /// <summary>
    /// 正常状态
    /// </summary>
    public static readonly CustomerStatus Normal = new CustomerStatus(100, "正常", "正常");

    /// <summary>
    /// 冻结状态
    /// </summary>
    public static readonly CustomerStatus Frozen = new CustomerStatus(200, "冻结", "冻结");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerStatus None = new CustomerStatus(0, "", "");

    public static CustomerStatus GetById(int id)
    {
        IList<CustomerStatus> statusList = GetList();
        foreach (CustomerStatus status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<CustomerStatus> GetList()
    {
        IList<CustomerStatus> statusList = new List<CustomerStatus>();
        statusList.Add(Normal);
        statusList.Add(Frozen);

        return statusList;
    }

    private CustomerStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerStatusHibernateType : CodeHibernateTypeBase<CustomerStatus>
{
    protected override CustomerStatus GetCodeTypeById(int id)
    {
        return CustomerStatus.GetById(id);
    }
}

public class CustomerStatusModelBinder : CodeTypeModelBinderBase<CustomerStatus>
{
    protected override CustomerStatus GetCodeTypeById(int id)
    {
        return CustomerStatus.GetById(id);
    }
}