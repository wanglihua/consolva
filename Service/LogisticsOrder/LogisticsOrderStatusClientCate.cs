using shunda.Core.CodeType;

namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderStatusClientCate : CodeTypeBase {
    /// <summary>
    /// 1000, 待支付
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate Pay = new LogisticsOrderStatusClientCate(1000, "待支付", "待支付");

    /// <summary>
    /// 2000, 待打包
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate Submitted = new LogisticsOrderStatusClientCate(2000, "待打包", "待打包");

    /// <summary>
    /// 3000, 待转运
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate WaitingTransport = new LogisticsOrderStatusClientCate(3000, "待转运", "待转运");

    /// <summary>
    /// 4000, 转运中
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate Transporting = new LogisticsOrderStatusClientCate(4000, "转运中", "转运中");

    /// <summary>
    /// 5000, 已交付
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate Delivered = new LogisticsOrderStatusClientCate(5000, "已交付", "已交付");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderStatusClientCate None = new LogisticsOrderStatusClientCate(0, "", "");

    private static readonly IReadOnlyList<LogisticsOrderStatusClientCate> codeTypeList = new List<LogisticsOrderStatusClientCate>() {
        Pay,
        Submitted,
        WaitingTransport,
        Transporting,
        Delivered,
    };

    private static readonly IDictionary<int, LogisticsOrderStatusClientCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderStatusClientCate GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderStatusClientCate? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderStatusClientCate> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsOrderStatusClientCate(int id, string name, string description) : base(id, name, description) {
    }
}