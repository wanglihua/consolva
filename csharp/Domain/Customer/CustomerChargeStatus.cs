using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户充值状态
/// </summary>
public class CustomerChargeStatus : CodeTypeBase
{
    /// <summary>
    /// 新充值状态
    /// </summary>
    public static readonly CustomerChargeStatus New = new CustomerChargeStatus(100, "新充值", "新充值");

    /// <summary>
    /// 已完成状态
    /// </summary>
    public static readonly CustomerChargeStatus Complete = new CustomerChargeStatus(200, "已完成", "已完成");

    /// <summary>
    /// None
    /// </summary>
    private static readonly CustomerChargeStatus None = new CustomerChargeStatus(0, "", "");

    private static readonly List<CustomerChargeStatus> codeTypeList = new List<CustomerChargeStatus>() {
        New,
        Complete,
    };

    private static readonly Dictionary<int, CustomerChargeStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerChargeStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerChargeStatus? codeType) == true)
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerChargeStatus> GetList()
    {
        return codeTypeList;
    }

    private CustomerChargeStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerChargeStatusHibernateType : CodeHibernateTypeBase<CustomerChargeStatus>
{
    protected override CustomerChargeStatus GetCodeTypeById(int id)
    {
        return CustomerChargeStatus.GetById(id);
    }
}

public class CustomerChargeStatusModelBinder : CodeTypeModelBinderBase<CustomerChargeStatus>
{
    protected override CustomerChargeStatus GetCodeTypeById(int id)
    {
        return CustomerChargeStatus.GetById(id);
    }
}