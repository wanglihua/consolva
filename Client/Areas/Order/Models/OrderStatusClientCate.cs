using shunda.Core.CodeType;

namespace shunda.Client.Areas.Order.Models;

public class OrderStatusClientCate : CodeTypeBase {
    /// <summary>
    /// 1000, 待支付
    /// </summary>
    public static readonly OrderStatusClientCate Pay = new OrderStatusClientCate(1000, "待支付", "待支付");

    /// <summary>
    /// 2000, 待打包
    /// </summary>
    public static readonly OrderStatusClientCate Submitted = new OrderStatusClientCate(2000, "待打包", "待打包");

    /// <summary>
    /// 3000, 待转运
    /// </summary>
    public static readonly OrderStatusClientCate WaitingTransport = new OrderStatusClientCate(3000, "待转运", "待转运");

    /// <summary>
    /// 4000, 转运中
    /// </summary>
    public static readonly OrderStatusClientCate Transporting = new OrderStatusClientCate(4000, "转运中", "转运中");

    /// <summary>
    /// 5000, 已交付
    /// </summary>
    public static readonly OrderStatusClientCate Delivered = new OrderStatusClientCate(5000, "已交付", "已交付");

    /// <summary>
    /// None
    /// </summary>
    public static readonly OrderStatusClientCate None = new OrderStatusClientCate(0, "", "");

    private static readonly IReadOnlyList<OrderStatusClientCate> codeTypeList = new List<OrderStatusClientCate>() {
        Pay,
        Submitted,
        WaitingTransport,
        Transporting,
        Delivered,
    };

    private static readonly IDictionary<int, OrderStatusClientCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static OrderStatusClientCate GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out OrderStatusClientCate? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<OrderStatusClientCate> GetList() {
        return codeTypeList.ToList();
    }

    private OrderStatusClientCate(int id, string name, string description) : base(id, name, description) {
    }
}