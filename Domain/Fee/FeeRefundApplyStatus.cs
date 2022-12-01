using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 退赔款申请状态
/// </summary>
public class FeeRefundApplyStatus : CodeTypeBase
{
    /// <summary>
    /// 未提交
    /// </summary>
    public static readonly FeeRefundApplyStatus Saved = new FeeRefundApplyStatus(100, "未提交", "未提交");

    /// <summary>
    /// 已回退
    /// </summary>
    public static readonly FeeRefundApplyStatus Backed = new FeeRefundApplyStatus(200, "已回退", "已回退");

    /// <summary>
    /// 已提交
    /// </summary>
    public static readonly FeeRefundApplyStatus Submitted = new FeeRefundApplyStatus(300, "已提交", "已提交");

    /// <summary>
    /// 已审核
    /// </summary>
    public static readonly FeeRefundApplyStatus Approved = new FeeRefundApplyStatus(400, "已审核", "已审核");


    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeRefundApplyStatus None = new FeeRefundApplyStatus(0, "", "");

    public static FeeRefundApplyStatus GetById(int id)
    {
        IList<FeeRefundApplyStatus> statusList = GetList();
        foreach (FeeRefundApplyStatus status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<FeeRefundApplyStatus> GetList()
    {
        IList<FeeRefundApplyStatus> statusList = new List<FeeRefundApplyStatus>();
        statusList.Add(Saved);
        statusList.Add(Backed);
        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    public static IList<FeeRefundApplyStatus> GetApproveViewList()
    {
        IList<FeeRefundApplyStatus> statusList = new List<FeeRefundApplyStatus>();
        statusList.Add(Submitted);
        statusList.Add(Approved);

        return statusList;
    }

    private FeeRefundApplyStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeRefundApplyStatusHibernateType : CodeHibernateTypeBase<FeeRefundApplyStatus>
{
    protected override FeeRefundApplyStatus GetCodeTypeById(int id)
    {
        return FeeRefundApplyStatus.GetById(id);
    }
}

public class FeeRefundApplyStatusModelBinder : CodeTypeModelBinderBase<FeeRefundApplyStatus>
{
    protected override FeeRefundApplyStatus GetCodeTypeById(int id)
    {
        return FeeRefundApplyStatus.GetById(id);
    }
}