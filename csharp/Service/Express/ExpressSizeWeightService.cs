using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressSizeWeightService
{
    private ISession session;

    public ExpressSizeWeightService(ISession session)
    {
        this.session = session;
    }

    public IList<ExpressSizeWeightInfo> GetExpressSizeWeightList(string trackingNo)
    {
        return session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
    }

    public IList<ExpressSizeWeightInfo> GetExpressSizeWeightList(IList<string> trackingNoList)
    {
        if (trackingNoList == null || trackingNoList.Count == 0)
        {
            return new List<ExpressSizeWeightInfo>();
        }

        return session.Query<ExpressSizeWeightInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();
    }
}