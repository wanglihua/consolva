using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单状态
/// </summary>
public class LogisticsOrderStatus : CodeTypeBase {
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
    /// 待发货
    /// </summary>
    public static readonly LogisticsOrderStatus WaitShip = new LogisticsOrderStatus(4000, "待发货", "待发货");

    /// <summary>
    /// 已发货
    /// </summary>
    public static readonly LogisticsOrderStatus Shipped = new LogisticsOrderStatus(5000, "已发货", "已发货");

    /// <summary>
    /// 已妥投
    /// </summary>
    public static readonly LogisticsOrderStatus Delivered = new LogisticsOrderStatus(6000, "已妥投", "已妥投");

    /// <summary>
    /// 正常结束
    /// </summary>
    public static readonly LogisticsOrderStatus Completed = new LogisticsOrderStatus(88_0000, "正常结束", "正常结束");

    /// <summary>
    /// 已作废
    /// </summary>
    public static readonly LogisticsOrderStatus Canceled = new LogisticsOrderStatus(90_0000, "已作废", "已作废");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderStatus None = new LogisticsOrderStatus(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderStatus> codeTypeList = new List<LogisticsOrderStatus>() {
        Create,
        WaitPack,
        Packed,
        WaitShip,
        Shipped,
        Delivered,
        Completed,
        Canceled,
    };

    private static readonly Dictionary<int, LogisticsOrderStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderStatus> GetList() {
        return codeTypeList.ToList();
    }

    /// <summary>
    /// 活动订单 状态列表
    /// 不是已完成，不是已作废
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsOrderStatus> GetActiveList() {
        return codeTypeList.Where(e => e != Completed && e != Canceled).ToList();
    }

    public static IList<LogisticsOrderStatus> GetBeforeSubmitPackStatusList() {
        return new List<LogisticsOrderStatus>() {
            Create,
        };
    }

    public static IList<LogisticsOrderStatus> GetAdminCanCancelStatusList() {
        return new List<LogisticsOrderStatus>() {
            Create,
            WaitPack,
            Packed,
        };
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    private LogisticsOrderStatus(int id, string name, string description) : base(id, name, description) {
    }

    /// <summary>
    /// 是否已经打包
    /// </summary>
    public bool HasPacked() {
        if (this == Create) {
            return false;
        }

        if (this == WaitPack) {
            return false;
        }

        return true;
    }
}

public class LogisticsOrderStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderStatus> {
    protected override LogisticsOrderStatus GetCodeTypeById(int id) {
        return LogisticsOrderStatus.GetById(id);
    }
}

public class LogisticsOrderStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderStatus> {
    protected override LogisticsOrderStatus GetCodeTypeById(int id) {
        return LogisticsOrderStatus.GetById(id);
    }
}