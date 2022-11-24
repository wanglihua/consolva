using NHibernate;
using shunda.Core;
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

    /// <summary>
    /// 汇总单个快递单号尺寸体积重量等
    /// </summary>
    /// <param name="expressId"></param>
    public void SumAndUpdateExpress(int expressId)
    {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();

        decimal totalVolume = 0m;
        decimal totalWeight = 0m;
        int packageCount = 0;

        foreach (var sizeWeight in sizeWeightList)
        {
            totalVolume += (sizeWeight.Length * sizeWeight.Width * sizeWeight.Height) * sizeWeight.PackageCount / 100_00_00;
            totalWeight += sizeWeight.Weight * sizeWeight.PackageCount;
            packageCount += sizeWeight.PackageCount;
        }

        express.TotalVolume = totalVolume;
        express.TotalWeight = totalWeight;
        express.PackageCount = packageCount; // 这样包裹数量就完全依赖于尺寸重量里面的数据了

        session.Update(express);
        session.Flush();
    }
}