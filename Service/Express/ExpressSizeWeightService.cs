using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Service.Logistics;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressSizeWeightService {
    private readonly ISession session;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressSizeWeightService(ISession session,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    public IList<ExpressSizeWeightInfo> GetOrderSizeWeightList(int orderId) {
        var expressList = session.Query<ExpressInfo>().Where(e => e.Order!.Id == orderId).ToList();
        var trackingNoList = expressList.Select(e => e.TrackingNo).ToList();

        return GetSizeWeightList(trackingNoList);
    }

    public IList<ExpressSizeWeightInfo> GetSizeWeightList(string trackingNo) {
        if (string.IsNullOrWhiteSpace(trackingNo)) {
            throw new ValidationException("快递单号不能为空！");
        }

        return GetSizeWeightList(new List<string> {
            trackingNo
        });
    }

    public IList<ExpressSizeWeightInfo> GetSizeWeightList(IList<string> trackingNoList) {
        if (trackingNoList == null || trackingNoList.Count == 0) {
            return new List<ExpressSizeWeightInfo>();
        }

        return session.Query<ExpressSizeWeightInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .OrderBy(e => e.Id) // 这个顺序是固定的，后面有逻辑依赖于这个顺序
            .ToList();
    }

    /// <summary>
    /// 获取总的包裹数量
    /// </summary>
    public int GetTotalPackageCount(IList<ExpressSizeWeightInfo> sizeWeightList) {
        if (sizeWeightList.Count == 0) {
            return 0;
        }

        return sizeWeightList.Sum(e => e.PackageCount);
    }

    /// <summary>
    /// 获取总的体积，单位是立方米 
    /// </summary>
    public decimal GetTotalVolume(IList<ExpressSizeWeightInfo> sizeWeightList) {
        if (sizeWeightList.Count == 0) {
            return 0;
        }

        return sizeWeightList.Sum(e => e.Length * e.Width * e.Height * e.PackageCount) / 100_00_00;
    }

    /// <summary>
    /// 获取总的体积重，单位是KG 
    /// </summary>
    public decimal GetTotalVolumeWeight(IList<ExpressSizeWeightInfo> sizeWeightList) {
        if (sizeWeightList.Count == 0) {
            return 0;
        }

        return sizeWeightList.Sum(e => e.Length * e.Width * e.Height * e.PackageCount) / logisticsModeSettingService.GetSetting().VolumeWeightDenominator; // 这里只能使用默认的体积重分母了
    }

    /// <summary>
    /// 获取总的实际重，单位是KG 
    /// </summary>
    public decimal GetTotalActualWeight(IList<ExpressSizeWeightInfo> sizeWeightList) {
        if (sizeWeightList.Count == 0) {
            return 0;
        }

        return sizeWeightList.Sum(e => e.Weight * e.PackageCount);
    }

    /// <summary>
    /// 获取最长边，单位是cm
    /// </summary>
    public decimal GetMaxSizeLength(IList<ExpressSizeWeightInfo> sizeWeightList) {
        if (sizeWeightList.Count == 0) {
            return 0;
        }

        return sizeWeightList.Max(e => Math.Max(Math.Max(e.Width, e.Length), e.Height));
    }

    /// <summary>
    /// 绑定快递单并合计更新快递单合计
    /// </summary>
    /// <param name="expressId"></param>
    /// <exception cref="ValidationException"></exception>
    public void BindExpressByTrackingNo(int expressId) {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

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
}