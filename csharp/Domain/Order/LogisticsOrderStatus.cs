using shunda.Core.CodeType;

namespace shunda.Domain.Order;

/// <summary>
/// 物流订单状态
/// </summary>
public class LogisticsOrderStatus : CodeTypeBase
{
    /// <summary>
    /// 已创建
    /// </summary>
    public static readonly LogisticsOrderStatus Create = new LogisticsOrderStatus(1000, "已创建", "已创建");

    /// <summary>
    /// 待打包
    /// </summary>
    public static readonly LogisticsOrderStatus WaitPack = new LogisticsOrderStatus(2000, "待打包", "待打包");

    /// <summary>
    /// 已打包
    /// </summary>
    public static readonly LogisticsOrderStatus Packed = new LogisticsOrderStatus(3000, "已打包", "已打包");

    /// <summary>
    /// 等待转运
    /// </summary>
    public static readonly LogisticsOrderStatus WaitTransfer = new LogisticsOrderStatus(4000, "待转运", "待转运");

    /// <summary>
    /// 已转运
    /// </summary>
    public static readonly LogisticsOrderStatus Transferred = new LogisticsOrderStatus(5000, "已转运", "已转运");

    /// <summary>
    /// 已妥投
    /// </summary>
    public static readonly LogisticsOrderStatus Delivered = new LogisticsOrderStatus(6000, "已妥投", "已妥投");

    /// <summary>
    /// 已作废
    /// </summary>
    public static readonly LogisticsOrderStatus Canceled = new LogisticsOrderStatus(9_8000, "已作废", "已作废");

    /// <summary>
    /// 异常结束
    /// </summary>
    public static readonly LogisticsOrderStatus Abend = new LogisticsOrderStatus(9_9000, "异常结束", "异常结束");

    /// <summary>
    /// 正常结束
    /// </summary>
    public static readonly LogisticsOrderStatus Completed = new LogisticsOrderStatus(10_0000, "正常结束", "正常结束");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderStatus None = new LogisticsOrderStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderStatus> codeTypeList = new List<LogisticsOrderStatus>() {
        Create,
        WaitPack,
        Packed,
        WaitTransfer,
        Transferred,
        Delivered,
        Canceled,
        Abend,
        Completed,
    };

    private static readonly Dictionary<int, LogisticsOrderStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    public static IList<LogisticsOrderStatus> GetBeforeSubmitPackStatusList()
    {
        return new List<LogisticsOrderStatus>() {
            Create,
        };
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    private LogisticsOrderStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderStatus>
{
    protected override LogisticsOrderStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderStatus.GetById(id);
    }
}

public class LogisticsOrderStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderStatus>
{
    protected override LogisticsOrderStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderStatus.GetById(id);
    }
}