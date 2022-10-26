using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 增减核销费用原因状态
/// </summary>
public class ManualFeeReasonStatus : CodeTypeBase
{
    /// <summary>
    /// 已启用, 1000
    /// </summary>
    public static readonly ManualFeeReasonStatus Enabled = new ManualFeeReasonStatus(1000, "已启用", "已启用");

    /// <summary>
    /// 已停用, 2000
    /// </summary>
    public static readonly ManualFeeReasonStatus Disabled = new ManualFeeReasonStatus(2000, "已停用", "已停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ManualFeeReasonStatus None = new ManualFeeReasonStatus(0, "", "");


    private static readonly List<ManualFeeReasonStatus> codeTypeList = new List<ManualFeeReasonStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, ManualFeeReasonStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ManualFeeReasonStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ManualFeeReasonStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ManualFeeReasonStatus> GetList()
    {
        return codeTypeList;
    }

    private ManualFeeReasonStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ManualFeeReasonStatusHibernateType : CodeHibernateTypeBase<ManualFeeReasonStatus>
{
    protected override ManualFeeReasonStatus GetCodeTypeById(int id)
    {
        return ManualFeeReasonStatus.GetById(id);
    }
}

public class ManualFeeReasonStatusModelBinder : CodeTypeModelBinderBase<ManualFeeReasonStatus>
{
    protected override ManualFeeReasonStatus GetCodeTypeById(int id)
    {
        return ManualFeeReasonStatus.GetById(id);
    }
}