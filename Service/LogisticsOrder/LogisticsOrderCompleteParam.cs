namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderCompleteParam {
    /// <summary>
    /// 订单Id
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 如果没有操作用户就取 0 值
    /// </summary>
    public int UserId { get; set; }
}