namespace shunda.Domain.Pass;

/// <summary>
/// 授权资源 id 定义
/// 商城系统中的 CateId 都是大于 9999_9999 的
/// </summary>
public class PassRes
{
    /// <summary>
    /// 商城订单
    /// </summary>
    public class ShopOrder
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 1_0100_0000;

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All = new List<int> {
            OrderList,
        };

        /// <summary>
        /// 订单列表
        /// </summary>
        public const int OrderList = ShopOrder.CateId + 0100;
    }
}