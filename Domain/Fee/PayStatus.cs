using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 物流订单支付状态
/// </summary>
public class PayStatus : CodeTypeBase
{
    /// <summary>
    /// 未支付, 1000
    /// </summary>
    public static readonly PayStatus NotPaid = new PayStatus(1000, "未支付", "未支付");

    /// <summary>
    /// 已支付, 2000
    /// </summary>
    public static readonly PayStatus Paid = new PayStatus(2000, "已支付", "已支付");

    /// <summary>
    /// 部分支付, 3000
    /// </summary>
    public static readonly PayStatus PartPaid = new PayStatus(3000, "部分支付", "部分支付");

    /// <summary>
    /// 已退款, 4000
    /// </summary>
    public static readonly PayStatus Refund = new PayStatus(4000, "已退款", "已退款");

    /// <summary>
    /// 拒绝退款, 5000
    /// </summary>
    public static readonly PayStatus RefundRefuse = new PayStatus(5000, "拒绝退款", "拒绝退款");

    /// <summary>
    /// None
    /// </summary>
    public static readonly PayStatus None = new PayStatus(0, "", "");

    private static readonly IReadOnlyList<PayStatus> codeTypeList = new List<PayStatus>() {
        NotPaid,
        Paid,
        PartPaid,
        Refund,
        RefundRefuse,
    };

    private static readonly Dictionary<int, PayStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static PayStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out PayStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<PayStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private PayStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class PayStatusHibernateType : CodeHibernateTypeBase<PayStatus>
{
    protected override PayStatus GetCodeTypeById(int id)
    {
        return PayStatus.GetById(id);
    }
}

public class PayStatusModelBinder : CodeTypeModelBinderBase<PayStatus>
{
    protected override PayStatus GetCodeTypeById(int id)
    {
        return PayStatus.GetById(id);
    }
}