using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 退赔款状态
/// </summary>
public class FeeRefundStatus : CodeTypeBase
{
    /// <summary>
    /// 已启用
    /// </summary>
    public static readonly FeeRefundStatus NotPaid = new FeeRefundStatus(100, "未支付", "未支付");

    /// <summary>
    /// 已停用
    /// </summary>
    public static readonly FeeRefundStatus Paid = new FeeRefundStatus(200, "已支付", "已支付");


    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeRefundStatus None = new FeeRefundStatus(0, "", "");

    public static FeeRefundStatus GetById(int id)
    {
        IList<FeeRefundStatus> statusList = GetList();
        foreach (FeeRefundStatus status in statusList)
        {
            if (status.Id == id)
            {
                return status;
            }
        }

        return None;
    }

    public static IList<FeeRefundStatus> GetList()
    {
        IList<FeeRefundStatus> statusList = new List<FeeRefundStatus>();
        statusList.Add(NotPaid);
        statusList.Add(Paid);

        return statusList;
    }

    private FeeRefundStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeRefundStatusHibernateType : CodeHibernateTypeBase<FeeRefundStatus>
{
    protected override FeeRefundStatus GetCodeTypeById(int id)
    {
        return FeeRefundStatus.GetById(id);
    }
}

public class FeeRefundStatusModelBinder : CodeTypeModelBinderBase<FeeRefundStatus>
{
    protected override FeeRefundStatus GetCodeTypeById(int id)
    {
        return FeeRefundStatus.GetById(id);
    }
}