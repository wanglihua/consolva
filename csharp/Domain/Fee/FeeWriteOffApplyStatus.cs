using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class FeeWriteOffApplyStatus : CodeTypeBase
{
    /// <summary>
    /// 未提交
    /// </summary>
    public static readonly FeeWriteOffApplyStatus Saved = new FeeWriteOffApplyStatus(100, "未提交", "未提交");

    /// <summary>
    /// 已回退
    /// </summary>
    public static readonly FeeWriteOffApplyStatus Backed = new FeeWriteOffApplyStatus(200, "已回退", "已回退");

    /// <summary>
    /// 已提交
    /// </summary>
    public static readonly FeeWriteOffApplyStatus Submitted = new FeeWriteOffApplyStatus(300, "已提交", "已提交");

    /// <summary>
    /// 已审核
    /// </summary>
    public static readonly FeeWriteOffApplyStatus Approved = new FeeWriteOffApplyStatus(400, "已审核", "已审核");

    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeWriteOffApplyStatus None = new FeeWriteOffApplyStatus(0, "", "");

    public static FeeWriteOffApplyStatus GetById(int id)
    {
        IList<FeeWriteOffApplyStatus> statusList = GetList();

        foreach (var status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<FeeWriteOffApplyStatus> GetList()
    {
        IList<FeeWriteOffApplyStatus> statusList = new List<FeeWriteOffApplyStatus>();

        statusList.Add(Saved);
        statusList.Add(Backed);
        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    public static IList<FeeWriteOffApplyStatus> GetApproveViewList()
    {
        IList<FeeWriteOffApplyStatus> statusList = new List<FeeWriteOffApplyStatus>();

        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    private FeeWriteOffApplyStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeWriteOffApplyStatusHibernateType : CodeHibernateTypeBase<FeeWriteOffApplyStatus>
{
    protected override FeeWriteOffApplyStatus GetCodeTypeById(int id)
    {
        return FeeWriteOffApplyStatus.GetById(id);
    }
}

public class FeeWriteOffApplyStatusModelBinder : CodeTypeModelBinderBase<FeeWriteOffApplyStatus>
{
    protected override FeeWriteOffApplyStatus GetCodeTypeById(int id)
    {
        return FeeWriteOffApplyStatus.GetById(id);
    }
}