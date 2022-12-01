using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsFeeRateService
{
    /// <summary>
    /// 获取体积重分母默认值
    /// </summary>
    /// <returns></returns>
    public int GetVolumeDenominator()
    {
        return LogisticsConfig.VolumeWeightDenominator;
    }
}
