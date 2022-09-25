namespace shunda.Domain.Pass;

/// <summary>
/// 授权资源 id 定义
/// </summary>
public class PassRes
{
    /// <summary>
    /// 仓储管理
    /// </summary>
    public class ShopOrder
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 01000_01000;

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All = new List<int> {
            OrderList,
        };

        /// <summary>
        /// 订单列表
        /// </summary>
        public const int OrderList = CateId + 0100;
    }
}