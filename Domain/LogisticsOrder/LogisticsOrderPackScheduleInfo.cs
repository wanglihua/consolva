using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackScheduleInfo : EntityNoUpdate {
    /// <summary>
    /// 打包工
    /// </summary>
    public virtual UserInfo User { get; set; } = new();

    /// <summary>
    /// 订单
    /// </summary>
    public virtual LogisticsOrderInfo Order { get; set; } = new();
}