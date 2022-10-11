using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 物流订单支付状态
/// </summary>
public class LogisticsOrderPayStatus : CodeTypeBase
{
    /// <summary>
    /// 未支付, 1000
    /// </summary>
    public static readonly LogisticsOrderPayStatus NotPaid = new LogisticsOrderPayStatus(1000, "未支付", "未支付");

    /// <summary>
    /// 已支付, 2000
    /// </summary>
    public static readonly LogisticsOrderPayStatus Paid = new LogisticsOrderPayStatus(2000, "已支付", "已支付");

    /// <summary>
    /// 部分支付, 3000
    /// </summary>
    public static readonly LogisticsOrderPayStatus PartPaid = new LogisticsOrderPayStatus(3000, "部分支付", "部分支付");

    /// <summary>
    /// 已退款, 4000
    /// </summary>
    public static readonly LogisticsOrderPayStatus Refund = new LogisticsOrderPayStatus(4000, "已退款", "已退款");

    /// <summary>
    /// 拒绝退款, 5000
    /// </summary>
    public static readonly LogisticsOrderPayStatus RefundRefuse = new LogisticsOrderPayStatus(5000, "拒绝退款", "拒绝退款");

    /// <summary>
    /// None
    /// </summary>
    private static readonly LogisticsOrderPayStatus None = new LogisticsOrderPayStatus(0, "", "");

    private static readonly List<LogisticsOrderPayStatus> codeTypeList = new List<LogisticsOrderPayStatus>() {
        NotPaid,
        Paid,
        PartPaid,
        Refund,
        RefundRefuse,
    };

    private static readonly Dictionary<int, LogisticsOrderPayStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderPayStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderPayStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderPayStatus> GetList()
    {
        return codeTypeList;
    }

    private LogisticsOrderPayStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderPayStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderPayStatus>
{
    protected override LogisticsOrderPayStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderPayStatus.GetById(id);
    }
}

public class LogisticsOrderPayStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderPayStatus>
{
    protected override LogisticsOrderPayStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderPayStatus.GetById(id);
    }
}