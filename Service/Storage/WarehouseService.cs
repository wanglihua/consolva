using shunda.Core.Service;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class WarehouseService {
    private readonly ISession session;

    public WarehouseService(ISession session) {
        this.session = session;
    }

    public IList<WarehouseInfo> GetWarehouseList() {
        return session.Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();
    }
}