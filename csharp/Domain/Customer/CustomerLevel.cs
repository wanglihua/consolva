using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户级别
/// </summary>
public class CustomerLevel : CodeTypeBase
{
    /// <summary>
    /// 普通会员, 1000
    /// </summary>
    public static readonly CustomerLevel Common = new CustomerLevel(1000, "普通会员", "普通会员");

    /// <summary>
    /// VIP会员, 2000
    /// </summary>
    public static readonly CustomerLevel Vip = new CustomerLevel(2000, "VIP会员", "VIP会员");

    /// <summary>
    /// 超级VIP, 3000
    /// </summary>
    public static readonly CustomerLevel SuperVip = new CustomerLevel(3000, "超级VIP", "超级VIP");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerLevel None = new CustomerLevel(0, "", "");

    public static CustomerLevel GetById(int id)
    {
        IList<CustomerLevel> customerLevelList = GetList();
        foreach (CustomerLevel customerLevel in customerLevelList)
        {
            if (customerLevel.Id == id)
            {
                return customerLevel;
            }
        }

        return None;
    }

    public static IList<CustomerLevel> GetList()
    {
        IList<CustomerLevel> customerLevelList = new List<CustomerLevel>();
        customerLevelList.Add(Common);
        customerLevelList.Add(Vip);
        customerLevelList.Add(SuperVip);

        return customerLevelList;
    }

    private CustomerLevel(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerLevelHibernateType : CodeHibernateTypeBase<CustomerLevel>
{
    protected override CustomerLevel GetCodeTypeById(int id)
    {
        return CustomerLevel.GetById(id);
    }
}

public class CustomerLevelModelBinder : CodeTypeModelBinderBase<CustomerLevel>
{
    protected override CustomerLevel GetCodeTypeById(int id)
    {
        return CustomerLevel.GetById(id);
    }
}