namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单创建选择快递单号记录
/// 跟原系统中的同名类是两个意思
/// </summary>
public class LogisticsOrderCreateSelectExpressInfo
{
    /// <summary>
    /// 客户，非空
    /// </summary>
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public virtual int ExpressId { get; set; }
}