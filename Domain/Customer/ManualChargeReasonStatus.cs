using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 手动充值原因状态
/// </summary>
public class ManualChargeReasonStatus : CodeTypeBase
{
    /// <summary>
    /// 已启用
    /// </summary>
    public static readonly ManualChargeReasonStatus Enabled = new ManualChargeReasonStatus(100, "已启用", "已启用");

    /// <summary>
    /// 已停用
    /// </summary>
    public static readonly ManualChargeReasonStatus Disabled = new ManualChargeReasonStatus(200, "已停用", "已停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ManualChargeReasonStatus None = new ManualChargeReasonStatus(0, "", "");

    private static readonly IReadOnlyList<ManualChargeReasonStatus> codeTypeList = new List<ManualChargeReasonStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, ManualChargeReasonStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ManualChargeReasonStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ManualChargeReasonStatus? codeType) == true)
        {
            return codeType;
        }

        return None;
    }

    public static IList<ManualChargeReasonStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private ManualChargeReasonStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ManualChargeReasonStatusHibernateType : CodeHibernateTypeBase<ManualChargeReasonStatus>
{
    protected override ManualChargeReasonStatus GetCodeTypeById(int id)
    {
        return ManualChargeReasonStatus.GetById(id);
    }
}

public class ManualChargeReasonStatusModelBinder : CodeTypeModelBinderBase<ManualChargeReasonStatus>
{
    protected override ManualChargeReasonStatus GetCodeTypeById(int id)
    {
        return ManualChargeReasonStatus.GetById(id);
    }
}