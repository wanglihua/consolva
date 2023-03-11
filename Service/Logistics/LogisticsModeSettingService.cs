using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

/// <summary>
/// 物流配置 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsModeSettingService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeSettingService(ISession session) {
        this.session = session;
    }

    public LogisticsModeSettingInfo GetSetting() {
        var setting = session.Query<LogisticsModeSettingInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (setting == null) {
            setting = new LogisticsModeSettingInfo {
                VolumeWeightDenominator = 5000,
                HasFreePackageLimit = true,
                FreePackageLimit = 1,
                ExtraPackageFee = 20m,
                HasExtraPackageFeeCapAmount = true,
                ExtraPackageFeeCapAmount = 20m,
            };
        }

        // 做数据检查
        if (setting.VolumeWeightDenominator <= 0) {
            throw new ValidationException("物流设置中默认体积重分母未设置或值不正确！");
        }

        return setting;
    }
}