using System.Text;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderRemarkService {
    private readonly ISession session;

    public LogisticsOrderRemarkService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取订单备注列表
    /// 主要是统一了排序等等的
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public IList<LogisticsOrderRemarkInfo> GetOrderRemarkList(int orderId) {
        var orderRemarkList = session.Query<LogisticsOrderRemarkInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();
        return orderRemarkList;
    }

    /// <summary>
    /// 获取订单备注用于订单列表显示
    /// </summary>
    public string GetOrderRemarkForList(IList<LogisticsOrderRemarkInfo> orderRemarkList) {
        orderRemarkList = orderRemarkList.OrderBy(e => e.Type.Id).ToList(); // 按类型 id 排序
        int remarkCount = orderRemarkList.Count;

        if (remarkCount == 0) {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        int index = 0;
        foreach (var orderRemark in orderRemarkList) {
            string remark = orderRemark.Content.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            sb.Append($"{orderRemark.Type.Name}:{remark}");
            if (index < remarkCount - 1) {
                sb.Append("<br/>");
            }

            index++;
        }

        return sb.ToString();
    }
}