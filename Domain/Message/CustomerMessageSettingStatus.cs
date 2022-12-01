using shunda.Core.CodeType;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息启用状态
/// </summary>
public class CustomerMessageSettingStatus : CodeTypeBase
{
    /// <summary>
    /// 启用
    /// </summary>
    public static readonly CustomerMessageSettingStatus Enabled = new CustomerMessageSettingStatus(100, "启用", "启用");

    /// <summary>
    /// 停用
    /// </summary>
    public static readonly CustomerMessageSettingStatus Disabled = new CustomerMessageSettingStatus(200, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerMessageSettingStatus None = new CustomerMessageSettingStatus(0, "", "");

    private static readonly IReadOnlyList<CustomerMessageSettingStatus> codeTypeList = new List<CustomerMessageSettingStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly IDictionary<int, CustomerMessageSettingStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerMessageSettingStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerMessageSettingStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerMessageSettingStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerMessageSettingStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerMessageSettingStatusHibernateType : CodeHibernateTypeBase<CustomerMessageSettingStatus>
{
    protected override CustomerMessageSettingStatus GetCodeTypeById(int id)
    {
        return CustomerMessageSettingStatus.GetById(id);
    }
}

public class CustomerMessageSettingStatusModelBinder : CodeTypeModelBinderBase<CustomerMessageSettingStatus>
{
    protected override CustomerMessageSettingStatus GetCodeTypeById(int id)
    {
        return CustomerMessageSettingStatus.GetById(id);
    }
}