using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressCheckinConfigService {
    private readonly ISession session;

    public ExpressCheckinConfigService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取快递单入库配置
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public ExpressCheckinConfigInfo GetExpressCheckinConfig() {
        ExpressCheckinConfigInfo? expressCheckinConfig = session
            .Query<ExpressCheckinConfigInfo>()
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (expressCheckinConfig == null) {
            // expressCheckinConfig = new ExpressCheckinConfigInfo();
            throw new ValidationException("快递单入库配置不存在！");
        }

        return expressCheckinConfig;
    }

    /// <summary>
    /// 获取系统打印标签类型
    /// </summary>
    /// <returns></returns>
    public ExpressCheckinLabelType GetLabelType(ExpressCheckinConfigInfo expressCheckinConfig) {
        if (expressCheckinConfig.LabelW10H05Status == ExpressCheckinLabelConfigStatus.Enabled) {
            return ExpressCheckinLabelType.SizeW10H05;
        }

        if (expressCheckinConfig.LabelW05H05Status == ExpressCheckinLabelConfigStatus.Enabled) {
            return ExpressCheckinLabelType.SizeW05H05;
        }

        // 如果都没配置启用就用老的标签
        return ExpressCheckinLabelType.SizeW10H05;
    }

    /// <summary>
    /// 获取系统标签打印数量
    /// </summary>
    /// <returns></returns>
    public int GetPrintCount() {
        ExpressCheckinConfigInfo expressCheckinConfig = GetExpressCheckinConfig();
        return GetPrintCount(expressCheckinConfig);
    }

    /// <summary>
    /// 获取系统标签打印数量
    /// </summary>
    /// <returns></returns>
    public int GetPrintCount(ExpressCheckinConfigInfo expressCheckinConfig) {
        if (expressCheckinConfig.LabelW10H05Status == ExpressCheckinLabelConfigStatus.Enabled) {
            return expressCheckinConfig.LabelW10H05PrintCount;
        }

        if (expressCheckinConfig.LabelW05H05Status == ExpressCheckinLabelConfigStatus.Enabled) {
            return expressCheckinConfig.LabelW05H05PrintCount;
        }

        // 如果都没配置启用就用老的标签
        // return ExpressCheckinLabelType.SizeW10H05Old;
        return expressCheckinConfig.LabelW10H05PrintCount;
    }
}