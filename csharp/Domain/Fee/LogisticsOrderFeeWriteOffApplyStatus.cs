using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class LogisticsOrderFeeWriteOffApplyStatus : CodeTypeBase
{
    /// <summary>
    /// 未提交
    /// </summary>
    public static readonly LogisticsOrderFeeWriteOffApplyStatus Saved = new LogisticsOrderFeeWriteOffApplyStatus(100, "未提交", "未提交");

    /// <summary>
    /// 已回退
    /// </summary>
    public static readonly LogisticsOrderFeeWriteOffApplyStatus Backed = new LogisticsOrderFeeWriteOffApplyStatus(200, "已回退", "已回退");

    /// <summary>
    /// 已提交
    /// </summary>
    public static readonly LogisticsOrderFeeWriteOffApplyStatus Submitted = new LogisticsOrderFeeWriteOffApplyStatus(300, "已提交", "已提交");

    /// <summary>
    /// 已审核
    /// </summary>
    public static readonly LogisticsOrderFeeWriteOffApplyStatus Approved = new LogisticsOrderFeeWriteOffApplyStatus(400, "已审核", "已审核");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderFeeWriteOffApplyStatus None = new LogisticsOrderFeeWriteOffApplyStatus(0, "", "");

    public static LogisticsOrderFeeWriteOffApplyStatus GetById(int id)
    {
        IList<LogisticsOrderFeeWriteOffApplyStatus> statusList = GetList();

        foreach (var status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<LogisticsOrderFeeWriteOffApplyStatus> GetList()
    {
        IList<LogisticsOrderFeeWriteOffApplyStatus> statusList = new List<LogisticsOrderFeeWriteOffApplyStatus>();

        statusList.Add(Saved);
        statusList.Add(Backed);
        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    public static IList<LogisticsOrderFeeWriteOffApplyStatus> GetApproveViewList()
    {
        IList<LogisticsOrderFeeWriteOffApplyStatus> statusList = new List<LogisticsOrderFeeWriteOffApplyStatus>();

        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    private LogisticsOrderFeeWriteOffApplyStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderFeeWriteOffApplyStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderFeeWriteOffApplyStatus>
{
    protected override LogisticsOrderFeeWriteOffApplyStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeWriteOffApplyStatus.GetById(id);
    }
}

public class LogisticsOrderFeeWriteOffApplyStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderFeeWriteOffApplyStatus>
{
    protected override LogisticsOrderFeeWriteOffApplyStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeWriteOffApplyStatus.GetById(id);
    }
}