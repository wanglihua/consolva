using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsModeService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public LogisticsModeService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 检查物流方式是不是支持对应的物品类型
    /// 物流方式与物品类型相关的设置是不是都正确
    /// </summary>
    /// <param name="logisticsModeId">物流方式Id</param>
    /// <param name="goodsType">物品类型</param>
    public void CheckGoodsTypeSupport(int logisticsModeId, LogisticsGoodsType goodsType) {
        var logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);
        if (logisticsMode == null) {
            throw new ValidationException($"指定的物流方式#{logisticsModeId}不存在！");
        }

        session.Query<LogisticsModeGoodsTypeInfo>().Where(e => e.LogisticsMode.Id == logisticsModeId);
    }
}