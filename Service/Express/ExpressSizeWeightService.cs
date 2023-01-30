using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressSizeWeightService {
    private readonly ISession session;

    public ExpressSizeWeightService(ISession session) {
        this.session = session;
    }

    public void BindAndSum(int expressId) {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        BindAndSum(express);
    }

    public void BindAndSum(ExpressInfo express) {
        // if (string.IsNullOrWhiteSpace(trackingNo))
        // {
        //     throw new ValidationException("绑定尺寸重量，快递单号不能为空！");
        // }
        //
        // if (trackingNo != express.TrackingNo)
        // {
        //     throw new ValidationException($"快递单号#{trackingNo}跟快递单信息#{express.Id}不匹配！");
        // }

        string trackingNo = express.TrackingNo;

        var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        decimal totalVolume = 0m;
        decimal totalWeight = 0m;
        int packageCount = 0;

        foreach (ExpressSizeWeightInfo sizeWeight in sizeWeightList) {
            sizeWeight.Express = express;
            session.Update(sizeWeight);

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

    /// <summary>
    /// 汇总单个快递单号尺寸体积重量等
    /// </summary>
    /// <param name="expressId"></param>
    public void SumDataAndUpdateExpress(int expressId) {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();

        decimal totalVolume = 0m;
        decimal totalWeight = 0m;
        int packageCount = 0;

        foreach (var sizeWeight in sizeWeightList) {
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