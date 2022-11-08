using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class ManualFeeApplyStatus : CodeTypeBase
{
    /// <summary>
    /// 未提交
    /// </summary>
    public static readonly ManualFeeApplyStatus Saved = new ManualFeeApplyStatus(100, "未提交", "未提交");

    /// <summary>
    /// 已回退
    /// </summary>
    public static readonly ManualFeeApplyStatus Backed = new ManualFeeApplyStatus(200, "已回退", "已回退");

    /// <summary>
    /// 已提交
    /// </summary>
    public static readonly ManualFeeApplyStatus Submitted = new ManualFeeApplyStatus(300, "已提交", "已提交");

    /// <summary>
    /// 已审核
    /// </summary>
    public static readonly ManualFeeApplyStatus Approved = new ManualFeeApplyStatus(400, "已审核", "已审核");

    /// <summary>
    /// None
    /// </summary>
    public static readonly ManualFeeApplyStatus None = new ManualFeeApplyStatus(0, "", "");

    private static readonly IReadOnlyList<ManualFeeApplyStatus> codeTypeList = new List<ManualFeeApplyStatus>() {
        Saved,
        Backed,
        Submitted,
        Approved,
    };

    private static readonly Dictionary<int, ManualFeeApplyStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static ManualFeeApplyStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out ManualFeeApplyStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<ManualFeeApplyStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    public static IList<ManualFeeApplyStatus> GetApproveViewList()
    {
        return new List<ManualFeeApplyStatus>() {
            Submitted,
            Approved,
        };
    }

    private ManualFeeApplyStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class ManualFeeApplyStatusHibernateType : CodeHibernateTypeBase<ManualFeeApplyStatus>
{
    protected override ManualFeeApplyStatus GetCodeTypeById(int id)
    {
        return ManualFeeApplyStatus.GetById(id);
    }
}

public class ManualFeeApplyStatusModelBinder : CodeTypeModelBinderBase<ManualFeeApplyStatus>
{
    protected override ManualFeeApplyStatus GetCodeTypeById(int id)
    {
        return ManualFeeApplyStatus.GetById(id);
    }
}