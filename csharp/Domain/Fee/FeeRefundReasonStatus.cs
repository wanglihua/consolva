using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 退赔原因状态
/// </summary>
public class FeeRefundReasonStatus : CodeTypeBase
{
    /// <summary>
    /// 已启用
    /// </summary>
    public static readonly FeeRefundReasonStatus Enabled = new FeeRefundReasonStatus(100, "已启用", "已启用");

    /// <summary>
    /// 已停用
    /// </summary>
    public static readonly FeeRefundReasonStatus Disabled = new FeeRefundReasonStatus(200, "已停用", "已停用");


    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeRefundReasonStatus None = new FeeRefundReasonStatus(0, "", "");

    public static FeeRefundReasonStatus GetById(int id)
    {
        IList<FeeRefundReasonStatus> statusList = GetList();
        foreach (FeeRefundReasonStatus status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<FeeRefundReasonStatus> GetList()
    {
        IList<FeeRefundReasonStatus> statusList = new List<FeeRefundReasonStatus>();
        statusList.Add(Enabled);
        statusList.Add(Disabled);

        return statusList;
    }

    private FeeRefundReasonStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeRefundReasonStatusHibernateType : CodeHibernateTypeBase<FeeRefundReasonStatus>
{
    protected override FeeRefundReasonStatus GetCodeTypeById(int id)
    {
        return FeeRefundReasonStatus.GetById(id);
    }
}

public class FeeRefundReasonStatusModelBinder : CodeTypeModelBinderBase<FeeRefundReasonStatus>
{
    protected override FeeRefundReasonStatus GetCodeTypeById(int id)
    {
        return FeeRefundReasonStatus.GetById(id);
    }
}