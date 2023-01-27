using NHibernate.Linq;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressRemarkService {
    private readonly ISession session;

    public ExpressRemarkService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取快递单备注列表
    /// 主要是统一了排序等等的
    /// </summary>
    /// <param name="expressId"></param>
    /// <returns></returns>
    public IList<ExpressRemarkInfo> GetExpressRemarkList(int expressId) {
        var expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express.Id == expressId)
            .OrderBy(e => e.Id)
            .ToList();
        return expressRemarkList;
    }

    /// <summary>
    /// 获取订单下面的快递单列表的快递单备注
    /// </summary>
    public IList<ExpressRemarkInfo> GetOrderExpressRemarkList(int orderId) {
        IList<int> expressIdList = session.Query<ExpressInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Select(e => e.Id)
            .ToList();

        var expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Fetch(e => e.Express)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .OrderBy(e => e.Express.Id)
            .ThenBy(e => e.Id)
            .ToList();

        return expressRemarkList;
    }
}