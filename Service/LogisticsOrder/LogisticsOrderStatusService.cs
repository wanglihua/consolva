using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderStatusService {
    /// <summary>
    /// 获取订单列表背景颜色
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public string GetOrderListColor(LogisticsOrderStatus status) {
        if (status == LogisticsOrderStatus.Canceled) {
            return "#BFBFBF";
        }

        if (status == LogisticsOrderStatus.Create) {
            return "#92CDDC";
        }

        if (status == LogisticsOrderStatus.WaitPack) {
            return "#92CDDC";
        }

        if (status == LogisticsOrderStatus.Packed) {
            return "#B1A0C7";
        }

        if (status == LogisticsOrderStatus.WaitShip) {
            return "#C4D79B";
        }

        if (status == LogisticsOrderStatus.Shipped) {
            return "#C4D79B";
        }

        if (status == LogisticsOrderStatus.Delivered) {
            return "#FABF8F";
        }

        return "#FFFFFF";
    }
}