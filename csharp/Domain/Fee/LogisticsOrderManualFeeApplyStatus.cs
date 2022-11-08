using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class LogisticsOrderManualFeeApplyStatus : CodeTypeBase
{
    /// <summary>
    /// 未提交
    /// </summary>
    public static readonly LogisticsOrderManualFeeApplyStatus Saved = new LogisticsOrderManualFeeApplyStatus(100, "未提交", "未提交");

    /// <summary>
    /// 已回退
    /// </summary>
    public static readonly LogisticsOrderManualFeeApplyStatus Backed = new LogisticsOrderManualFeeApplyStatus(200, "已回退", "已回退");

    /// <summary>
    /// 已提交
    /// </summary>
    public static readonly LogisticsOrderManualFeeApplyStatus Submitted = new LogisticsOrderManualFeeApplyStatus(300, "已提交", "已提交");

    /// <summary>
    /// 已审核
    /// </summary>
    public static readonly LogisticsOrderManualFeeApplyStatus Approved = new LogisticsOrderManualFeeApplyStatus(400, "已审核", "已审核");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderManualFeeApplyStatus None = new LogisticsOrderManualFeeApplyStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderManualFeeApplyStatus> codeTypeList = new List<LogisticsOrderManualFeeApplyStatus>() {
        Saved,
        Backed,
        Submitted,
        Approved,
    };

    private static readonly Dictionary<int, LogisticsOrderManualFeeApplyStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderManualFeeApplyStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderManualFeeApplyStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderManualFeeApplyStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    public static IList<LogisticsOrderManualFeeApplyStatus> GetApproveViewList()
    {
        return new List<LogisticsOrderManualFeeApplyStatus>() {
            Submitted,
            Approved,
        };
    }

    private LogisticsOrderManualFeeApplyStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderManualFeeApplyStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderManualFeeApplyStatus>
{
    protected override LogisticsOrderManualFeeApplyStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderManualFeeApplyStatus.GetById(id);
    }
}

public class LogisticsOrderManualFeeApplyStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderManualFeeApplyStatus>
{
    protected override LogisticsOrderManualFeeApplyStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderManualFeeApplyStatus.GetById(id);
    }
}